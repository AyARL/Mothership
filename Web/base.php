<?php
 		session_start();
        $secretKey = "key";

        $db = mysql_connect('studentnet.kingston.ac.uk', 'k1159960', 'digiDb14') or die('Could not connect: ' . mysql_error()); 
        mysql_select_db('db_k1159960') or die('Could not select database');
?>