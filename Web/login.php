
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
       	    echo json_encode($row);

                $userID = $row['UserID'];
                $updateQuery = "UPDATE Profile SET `LastLogin` = CURRENT_TIMESTAMP WHERE `UserID` = '" .$userID. "';";
                mysql_query($updateQuery);
            }
            else
            {
                  echo LoginResponse::Error_IncorrectCredentials;
            }
      }
      else
      {
       	echo Response::Error_InvalidHash;
      }
?>