-- phpMyAdmin SQL Dump
-- version 5.1.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 02, 2021 at 04:17 PM
-- Server version: 10.5.9-MariaDB
-- PHP Version: 7.4.19

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `o3c_manager`
--

-- --------------------------------------------------------

--
-- Table structure for table `devices_dynamic`
--

CREATE TABLE `devices_dynamic` (
  `id` int(11) NOT NULL,
  `o3c_server` int(11) NOT NULL,
  `client_id` varchar(50) NOT NULL,
  `model` varchar(50) DEFAULT NULL,
  `firmware` varchar(20) DEFAULT NULL,
  `source_addr` varchar(50) DEFAULT NULL,
  `state` tinyint(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `devices_static`
--

CREATE TABLE `devices_static` (
  `id` int(11) NOT NULL,
  `client_id` varchar(50) NOT NULL,
  `o3c_server_dst` int(11) NOT NULL,
  `folder` int(11) NOT NULL DEFAULT 1,
  `display_name` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `folders`
--

CREATE TABLE `folders` (
  `id` int(11) NOT NULL,
  `parent_folder` int(11) NOT NULL,
  `name` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `folders`
--

INSERT INTO `folders` (`id`, `parent_folder`, `name`) VALUES
(1, 0, 'O3C Manager'),
(2, 1, 'My first folder'),
(3, 1, 'My second folder'),
(4, 2, 'Nested folder'),
(5, 4, 'Nested again');

-- --------------------------------------------------------

--
-- Table structure for table `global_settings`
--

CREATE TABLE `global_settings` (
  `setting` varchar(100) NOT NULL,
  `value` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `global_settings`
--

INSERT INTO `global_settings` (`setting`, `value`) VALUES
('o3c_entry_point', '1');

-- --------------------------------------------------------

--
-- Table structure for table `o3c_servers`
--

CREATE TABLE `o3c_servers` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `host` varchar(100) NOT NULL,
  `port` int(11) NOT NULL DEFAULT 8080,
  `state` tinyint(11) NOT NULL DEFAULT 1,
  `admin_port` int(11) NOT NULL DEFAULT 3128,
  `failover_o3c_id` int(11) DEFAULT NULL,
  `display_name` varchar(150) DEFAULT NULL,
  `host_external` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `devices_dynamic`
--
ALTER TABLE `devices_dynamic`
  ADD PRIMARY KEY (`client_id`) USING BTREE,
  ADD UNIQUE KEY `id` (`id`) USING BTREE,
  ADD KEY `o3c_server` (`o3c_server`) USING BTREE;

--
-- Indexes for table `devices_static`
--
ALTER TABLE `devices_static`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `device_dynamic_id` (`client_id`);

--
-- Indexes for table `folders`
--
ALTER TABLE `folders`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `global_settings`
--
ALTER TABLE `global_settings`
  ADD PRIMARY KEY (`setting`);

--
-- Indexes for table `o3c_servers`
--
ALTER TABLE `o3c_servers`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `devices_dynamic`
--
ALTER TABLE `devices_dynamic`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `devices_static`
--
ALTER TABLE `devices_static`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `folders`
--
ALTER TABLE `folders`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `o3c_servers`
--
ALTER TABLE `o3c_servers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
