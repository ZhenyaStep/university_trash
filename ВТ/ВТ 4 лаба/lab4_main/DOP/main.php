<?php

$HOST = '127.0.0.1';
$NAME = 'root';
$PASSWORD = '';
$DB_NAME  = 'users';


require 'BDWork.php';
include 'functions.php';

header('Content-Type: text/html; charset=utf-8');

$mysqli = new mysqli($HOST, $NAME, $PASSWORD, $DB_NAME);
if ($mysqli->connect_error) {die('error connection DB');}
else {
    $mysqli->query('SET names "utf8"');
    $vars_array = array(
        'key1' =>'Zhenya',
        'key2' => 'Anton'
    );
    $config_path = 'config.cfg';
    $db_table = $DB_NAME;
    $FILE = 'some_file.txt';
    echo replacing($FILE, $vars_array, $config_path, $db_table, $mysqli);
    $mysqli->close();
    echo "it's work";
}

