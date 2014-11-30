<?php include "base.php";
      include "error.php";
 ?>
<?php 
	$email = mysql_real_escape_string($_POST['Email'], $db);
	$passwordHash = mysql_real_escape_string($_POST['Password'], $db);
	$displayName = mysql_real_escape_string($_POST['DisplayName'], $db);
	$hash = mysql_real_escape_string($_POST['Hash'], $db);

	$real_hash = md5($email . $gameAppKey);

	if(!empty($email) && !empty($passwordHash) && !empty($displayName))
	{
		#Continue only if the hashes match
		if($hash == $real_hash)
		{
			mysql_query("SET AUTOCOMMIT=0");
			mysql_query("START TRANSACTION");

			$insertUser = "INSERT INTO User (Email, Password) VALUES ('" . $email . "', '" . $passwordHash . "');"; 
			if(mysql_query($insertUser))
			{
				$getUser = "SELECT * FROM User WHERE `Email` = '" . $email . "';";
				$result = mysql_query($getUser) or die($db->error);

				$row = mysql_fetch_array($result);
				$userID = $row['UserID'];

				$insertProfile = "INSERT INTO Profile (UserID, DisplayName) VALUES ('". $userID ."', '". $displayName ."');";
				if(mysql_query($insertProfile))
				{
					mysql_query("COMMIT");
					echo json_encode($row);
				}
				else
				{
					mysql_query("ROLLBACK");
					echo AccountCreationResponse::Error_NameInUse;
				}
			}
			else
			{
				mysql_query("ROLLBACK");
				echo AccountCreationResponse::Error_EmailInUse;
			}
		}
		else
		{
			echo AccountCreationResponse::Error_InvalidHash;
		}
	}

?>