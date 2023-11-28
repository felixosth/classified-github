-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Värd: s680.loopia.se
-- Tid vid skapande: 01 feb 2019 kl 16:29
-- Serverversion: 10.2.19-MariaDB-log
-- PHP-version: 7.2.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databas: `tryggconnect_se_db_1`
--

-- --------------------------------------------------------

--
-- Tabellstruktur `recipients`
--

CREATE TABLE `recipients` (
  `id` int(11) NOT NULL,
  `type` varchar(10) NOT NULL,
  `data` varchar(150) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumpning av Data i tabell `recipients`
--

INSERT INTO `recipients` (`id`, `type`, `data`) VALUES
(1, 'email', 'felix.osth@insupport.se'),
(2, 'sms', '+46701001474'),
(3, 'email', 'support@insupport.se');

-- --------------------------------------------------------

--
-- Tabellstruktur `sites`
--

CREATE TABLE `sites` (
  `id` int(11) NOT NULL,
  `url` varchar(200) NOT NULL,
  `lastCheck` datetime NOT NULL DEFAULT current_timestamp(),
  `lastOnline` datetime NOT NULL DEFAULT current_timestamp(),
  `alarmThreshold` int(11) NOT NULL DEFAULT 600,
  `alarmSent` tinyint(4) NOT NULL DEFAULT 0,
  `port` int(11) NOT NULL DEFAULT 80
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumpning av Data i tabell `sites`
--

INSERT INTO `sites` (`id`, `url`, `lastCheck`, `lastOnline`, `alarmThreshold`, `alarmSent`, `port`) VALUES
(2, 'drift.tryggconnect.se', '2019-02-01 16:25:01', '2019-02-01 16:25:01', 800, 0, 80),
(3, 'oneklick.tryggconnect.se', '2019-02-01 16:25:01', '2019-02-01 16:25:01', 800, 0, 8081);

--
-- Index för dumpade tabeller
--

--
-- Index för tabell `recipients`
--
ALTER TABLE `recipients`
  ADD PRIMARY KEY (`id`);

--
-- Index för tabell `sites`
--
ALTER TABLE `sites`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT för dumpade tabeller
--

--
-- AUTO_INCREMENT för tabell `recipients`
--
ALTER TABLE `recipients`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT för tabell `sites`
--
ALTER TABLE `sites`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
