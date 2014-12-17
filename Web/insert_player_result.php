INSERT INTO  `db_k1159960`.`GamePlayer` (
`GameID` ,
`UserID` ,
`Team` ,
`EXPEarned`
)
VALUES (
'3',  '2',  'Blue',  '50'
);

<?php include "base.php";
      include "error.php";
 ?>
<?php 
	$email = mysql_real_escape_string($_POST['Email'], $db);
    $password = mysql_real_escape_string($_POST['Password'], $db);
    $hash = mysql_real_escape_string($_POST['Hash'], $db);

      $real_hash = md5($email . $gameAppKey);

      if($hash == $real_hash)
      {
            $loginQuery = "SELECT * FROM User WHERE `Email` = '".$email."' AND `Password` = '".$password."';";
            $result = mysql_query($loginQuery);
            if(mysql_num_rows($result) == 1)
            {
            	$row = mysql_fetch_array($result);
				$userID = $row['UserID'];

				$updateQuery = "UPDATE Profile SET `LastLogin` = CURRENT_TIMESTAMP WHERE `UserID` = '" .$userID. "';";
                mysql_query($updateQuery);

       	    	$getProfileQuery = "SELECT * FROM Profile WHERE `UserID` = '" . $userID . "' ";
       	    	$profileResult = mysql_query($getProfileQuery);
       	    	if(mysql_num_rows($profileResult) == 1)
       	    	{
       	    		$row = mysql_fetch_array($profileResult);
       	    		echo json_encode($row);
       	    	}
       	    	else
       	    	{
       	    		echo GetProfileResponse::Error_NoProfile;
       	    	}
            }
            else
            {
                  echo GetProfileResponse::Error_IncorrectCredentials;
            }
      }
      else
      {
       	echo Response::Error_InvalidHash;
      }
?>