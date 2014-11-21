
<?php include "base.php"; ?>
<?php 
        $username = mysql_real_escape_string($_GET['uname'], $db);
        $password = mysql_real_escape_string($_GET['pass'], $db);
        $hash = $_GET['hash'];

       	$validHash = md5($userName . $password . $secretKey);

       	# TEMP FOR WEB TEST - DELETE THIS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
       	$hash = $validHash;

       	if($hash == $validHash)
       	{
       		$loginQuery = "SELECT * FROM `User` WHERE `Username` = '".$username."' AND `Pass` = '".$password."' ";
       		$result = mysql_query($loginQuery) or die('Login failed ' . mysql_error());
       		if(mysql_num_rows($result) == 1)
       		{
       			$row = mysql_fetch_array($result);
       			$_SESSION['Username'] = $row['Username'];
       			$_SESSION['LoggedIn'] = 1;

       			echo json_encode($row);



       			#PrintUserData($row);

       			#echo "LoginSuccesful";
       		}
       		else
       		{
       			echo "LoginFailed";
       		}
       	}
       	else
       	{
       		echo "InvalidHash";
       	}

       	function PrintUserData($dataRow)
       	{
       		$xmlstr = "<session></session>";
       		$xml = new SimpleXMLElement($xmlstr);

       		$user = $xml->addChild('user');
       		$user->addChild('userID', $dataRow['UserID']);
       		$user->addChild('username', $dataRow['Username']);

       		print($xml->asXML());
       	}
?>