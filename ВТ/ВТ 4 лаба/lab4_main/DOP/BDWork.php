<?php

$HOST = "127.0.0.1";
$USER = "root";
$PASSWORD = "";
$DB_NAME ='users';



if(!$mysqli = mysqli_connect($HOST, $USER, $PASSWORD)) {echo "problems with mysql  "; die();}


if(!$mysqli->query("CREATE DATABASE $DB_NAME"))

if(!$mysqli->select_db($DB_NAME)) {echo "can't select    ";}

$mysqli->query('SET NAMES cp1251;');

$name = "name";
$fam = "fam";
$mail = "mail";

$insert_sql = "INSERT INTO $DB_NAME (`first_name`, `last_name`, `email`) VALUES ('$name', '$fam', '$mail')";
$mysqli->query($insert_sql);


$result = $mysqli->query("SELECT * FROM $DB_NAME");


/*
//echo "<table>"; // start a table tag in the HTML

//while($row = $mysqli->($result)){   //Creates a loop to loop through results
//    echo "<tr><td>" . $row['first_name'] . "</td><td>" . $row['last_name'] . "</td></tr>";  //$row['index'] the index here is a field name
//}

//echo "</table>"; //Close the table in HTML

while($row = mysqli_fetch_row($result)) {

    // Записать значение столбца FirstName (который является теперь массивом $row)
    echo $row[0] . "<br />";

}
*/
echo "all good    \n";
