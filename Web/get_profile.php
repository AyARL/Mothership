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

                        $gamesPlayedQuery = "SELECT Count(*) as GamesPlayed FROM `GamePlayer` WHERE `UserID` = '" .$userID. "';";
                        $expQuery = "SELECT Sum(`EXPEarned`) as EXP FROM `GamePlayer` WHERE `UserID` = '" .$userID. "';";
                        $gamesWonQuery = "SELECT Count(*) as GamesWon FROM `GamePlayer` INNER JOIN `GameStats` ON GamePlayer.GameID = GameStats.GameID WHERE `UserID` = '" .$userID. "' AND GamePlayer.Team = GameStats.Winner;";
                        $gamesLostQuery = "SELECT Count(*) as GamesLost FROM `GamePlayer` INNER JOIN `GameStats` ON GamePlayer.GameID = GameStats.GameID WHERE `UserID` = 3 AND GamePlayer.Team != GameStats.Winner;";

                        $gamesPlayedResult = mysql_query($gamesPlayedQuery);
                        $expResult = mysql_query($expQuery);
                        $gamesWonResult = mysql_query($gamesWonQuery);
                        $gamesLostResult = mysql_query($gamesLostQuery);

                        $gamesPlayedRow = mysql_fetch_array($gamesPlayedResult);
                        $expRow = mysql_fetch_array($expResult);
                        $gamesWonRow = mysql_fetch_array($gamesWonResult);
                        $gamesLostRow = mysql_fetch_array($gamesLostResult);

                        $profileData = array("ProfileID" => $row['ProfileID'], 
                                             "UserID" => $row['UserID'],
                                             "DisplayName" => $row['DisplayName'], 
                                             "EXP" => $expRow['EXP'],
                                             "GamesPlayed" => $gamesPlayedRow['GamesPlayed'],
                                             "GamesWon" => $gamesWonRow['GamesWon'],
                                             "GamesLost" => $gamesLostRow['GamesLost'],
                                             "LastLogin" => $row['LastLogin']
                                             );
       	    		echo json_encode($profileData);
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
