-- phpMyAdmin SQL Dump
-- version 4.4.10
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1:3306
-- Generation Time: Feb 01, 2016 at 08:12 AM
-- Server version: 5.6.25-log
-- PHP Version: 5.6.8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `system_magazynowy`
--
CREATE DATABASE IF NOT EXISTS `system_magazynowy` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `system_magazynowy`;

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `dodaj_zamowienie`(IN `_osoba` TEXT, IN `_adres` TEXT, IN `_dostawa` INT, OUT `_id` INT)
BEGIN
	INSERT INTO zamowienia (data_utworzenia, osoba, adres, sposob_dostarczenia) VALUES (CURDATE(), _osoba, _adres, _dostawa);
	SELECT LAST_INSERT_ID() INTO _id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `dodaj_zamowienie_towary`(IN `_id_zamowienie` INT, IN `_id_towar` INT, IN `_ilosc` INT)
BEGIN
	INSERT INTO zamowienia_towary (zamowienia_id, towary_kod_towaru, ilosc) VALUES (_id_zamowienie, _id_towar, _ilosc);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `select_sposoby_dostarczenia_all`()
BEGIN
	SELECT * FROM sposoby_dostarczenia;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `select_towary`(IN _kod_towaru INT, IN _nazwa_towaru TEXT)
BEGIN
IF _kod_towaru = -1 THEN

SELECT towary.kod_towaru, towary.nazwa, DATE_FORMAT(towary.data_gwarancji,'%m-%d-%Y') AS 'data_gwarancji', towary.ilosc, typy_towaru.nazwa AS 'typ_towaru' FROM towary INNER JOIN typy_towaru ON towary.typ_towaru = typy_towaru.id AND towary.nazwa LIKE CONCAT('%', _nazwa_towaru, '%');

ELSE

SELECT towary.kod_towaru, towary.nazwa, DATE_FORMAT(towary.data_gwarancji,'%m-%d-%Y') AS 'data_gwarancji', towary.ilosc, typy_towaru.nazwa AS 'typ_towaru' FROM towary INNER JOIN typy_towaru ON towary.typ_towaru = typy_towaru.id AND towary.nazwa LIKE CONCAT('%', _nazwa_towaru, '%') AND towary.kod_towaru = _kod_towaru;

END IF;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `select_towary_all`()
BEGIN
	SELECT * FROM towary;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `select_towary_ilosc`(IN _kod_towaru INT)
BEGIN
	SELECT ilosc FROM towary WHERE kod_towaru = _kod_towaru;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `select_typy_towaru_all`()
BEGIN
	SELECT * FROM typy_towaru;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `select_zamowienia_all`()
BEGIN
	SELECT zamowienia.id, zamowienia.data_utworzenia, zamowienia.osoba, zamowienia.adres, zamowienia.data_realizacji, sposoby_dostarczenia.nazwa AS 'sposob_dostarczenia' FROM zamowienia LEFT JOIN sposoby_dostarczenia ON zamowienia.sposob_dostarczenia = sposoby_dostarczenia.id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `towary_ilosc`(IN _id_towar INT, IN _ilosc INT)
BEGIN
	UPDATE towary SET ilosc = ilosc - _ilosc WHERE kod_towaru = _id_towar;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `towary_zamowienia`(IN _id_zamowienia INT)
BEGIN
	SELECT towary.kod_towaru, towary.nazwa, zamowienia_towary.ilosc FROM zamowienia_towary INNER JOIN towary ON towary.kod_towaru = zamowienia_towary.towary_kod_towaru WHERE zamowienia_towary.zamowienia_id = _id_zamowienia;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `update_zamowienia`(IN _id INT, IN _data DATE)
BEGIN
	UPDATE zamowienia SET data_realizacji = _data WHERE id = _id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `zamowienia_towaru`(IN _kod_towaru INT)
BEGIN
	SELECT * FROM zamowienia_towary WHERE towary_kod_towaru = _kod_towaru;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `sposoby_dostarczenia`
--

CREATE TABLE IF NOT EXISTS `sposoby_dostarczenia` (
  `id` int(11) NOT NULL,
  `nazwa` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `sposoby_dostarczenia`
--

TRUNCATE TABLE `sposoby_dostarczenia`;
--
-- Dumping data for table `sposoby_dostarczenia`
--

INSERT INTO `sposoby_dostarczenia` (`id`, `nazwa`) VALUES
(1, 'List ekonomiczny'),
(2, 'List priorytetowy'),
(3, 'Przesyłka kurierska');

-- --------------------------------------------------------

--
-- Table structure for table `towary`
--

CREATE TABLE IF NOT EXISTS `towary` (
  `kod_towaru` int(11) NOT NULL,
  `nazwa` text CHARACTER SET utf8 NOT NULL,
  `data_gwarancji` datetime DEFAULT NULL,
  `typ_towaru` int(11) NOT NULL,
  `ilosc` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Truncate table before insert `towary`
--

TRUNCATE TABLE `towary`;
--
-- Dumping data for table `towary`
--

INSERT INTO `towary` (`kod_towaru`, `nazwa`, `data_gwarancji`, `typ_towaru`, `ilosc`) VALUES
(1, 'Pomidory', '2016-02-26 00:00:00', 3, 10),
(2, 'Marchewki', '2016-04-21 00:00:00', 4, 20),
(3, 'Banany', '2016-02-13 00:00:00', 2, 99),
(4, 'Pomarańcze', '2016-09-30 00:00:00', 1, 150);

-- --------------------------------------------------------

--
-- Table structure for table `typy_towaru`
--

CREATE TABLE IF NOT EXISTS `typy_towaru` (
  `id` int(11) NOT NULL,
  `nazwa` text NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `typy_towaru`
--

TRUNCATE TABLE `typy_towaru`;
--
-- Dumping data for table `typy_towaru`
--

INSERT INTO `typy_towaru` (`id`, `nazwa`) VALUES
(0, 'TYP 5'),
(1, 'TYP 1'),
(2, 'TYP 2'),
(3, 'TYP 3'),
(4, 'TYP 4');

-- --------------------------------------------------------

--
-- Table structure for table `zamowienia`
--

CREATE TABLE IF NOT EXISTS `zamowienia` (
  `id` int(11) NOT NULL,
  `data_utworzenia` date NOT NULL,
  `osoba` text NOT NULL,
  `adres` text NOT NULL,
  `data_realizacji` date DEFAULT NULL,
  `sposob_dostarczenia` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `zamowienia`
--

TRUNCATE TABLE `zamowienia`;
-- --------------------------------------------------------

--
-- Table structure for table `zamowienia_towary`
--

CREATE TABLE IF NOT EXISTS `zamowienia_towary` (
  `zamowienia_id` int(11) NOT NULL,
  `towary_kod_towaru` int(11) NOT NULL,
  `ilosc` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `zamowienia_towary`
--

TRUNCATE TABLE `zamowienia_towary`;
--
-- Indexes for dumped tables
--

--
-- Indexes for table `sposoby_dostarczenia`
--
ALTER TABLE `sposoby_dostarczenia`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `towary`
--
ALTER TABLE `towary`
  ADD PRIMARY KEY (`kod_towaru`),
  ADD KEY `fk_towary_typy_towaru_idx` (`typ_towaru`);

--
-- Indexes for table `typy_towaru`
--
ALTER TABLE `typy_towaru`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `zamowienia`
--
ALTER TABLE `zamowienia`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_zamowienia_sposoby_dostarczenia1_idx` (`sposob_dostarczenia`);

--
-- Indexes for table `zamowienia_towary`
--
ALTER TABLE `zamowienia_towary`
  ADD PRIMARY KEY (`zamowienia_id`,`towary_kod_towaru`),
  ADD KEY `fk_zamowienia_has_towary_towary1_idx` (`towary_kod_towaru`),
  ADD KEY `fk_zamowienia_has_towary_zamowienia1_idx` (`zamowienia_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `towary`
--
ALTER TABLE `towary`
  MODIFY `kod_towaru` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=5;
--
-- AUTO_INCREMENT for table `typy_towaru`
--
ALTER TABLE `typy_towaru`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=5;
--
-- AUTO_INCREMENT for table `zamowienia`
--
ALTER TABLE `zamowienia`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `towary`
--
ALTER TABLE `towary`
  ADD CONSTRAINT `fk_towary_typy_towaru` FOREIGN KEY (`typ_towaru`) REFERENCES `typy_towaru` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `zamowienia`
--
ALTER TABLE `zamowienia`
  ADD CONSTRAINT `fk_zamowienia_sposoby_dostarczenia1` FOREIGN KEY (`sposob_dostarczenia`) REFERENCES `sposoby_dostarczenia` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `zamowienia_towary`
--
ALTER TABLE `zamowienia_towary`
  ADD CONSTRAINT `fk_zamowienia_has_towary_towary1` FOREIGN KEY (`towary_kod_towaru`) REFERENCES `towary` (`kod_towaru`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_zamowienia_has_towary_zamowienia1` FOREIGN KEY (`zamowienia_id`) REFERENCES `zamowienia` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
