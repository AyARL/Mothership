<?php include "base.php";
      include "error.php";
 ?>
<?php 
	$winner = mysql_real_escape_string($_POST['Winner'], $db);
	$players = json_decode($_POST['Players'], true);
	$hash = mysql_real_escape_string($_POST['Hash'], $db);

    $real_hash = md5($winner . $gameAppKey);

    if($hash == $real_hash)
    {
    	$timestamp = date("Y-m-d H:i:s", time());
        $insertQuery = "INSERT INTO `GameStats` (`DatePlayed`, `Winner`) VALUES ('".$timestamp."', '". $winner ."');";
        $insertResult = mysql_query($insertQuery);
        if($insertResult)
        {
        	$gameQuery = "SELECT * FROM `GameStats` WHERE `DatePlayed` = '".$timestamp."' AND `Winner` = '".$winner."';";
        	$gameResult = mysql_query($gameQuery);
        	if(mysql_num_rows($gameResult) > 0)
            {
            	$row = mysql_fetch_array($gameResult);
            	foreach ($players as $value) 
            	{
            		$playerQuery = "INSERT INTO `GamePlayer` (`GameID` ,`UserID` ,`Team` ,`EXPEarned`)VALUES ('".$row['GameID']."',  '".$value["UserID"]."',  '".$value["Team"]."',  '".$value["EXP"]."')";
            		$playerInsertResult = mysql_query($playerQuery);
            		if(!$playerInsertResult)
            		{
            			echo InsertGameResponse::Error_PlayerInsertFailed;
            		}
            	}
            }
        }
        else
        {
        	echo InsertGameResponse::Error_GameInsertFailed;
        }
    }
    else
    {
       	echo Response::Error_InvalidHash;
    }
?>