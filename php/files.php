<?php
    require "db.php";
    
//    $query = "select * from RouteTrackFiles";
//    $res = @mysql_query($query);
//    echo "Files:" . "</br>";
//    while ($i = @mysql_fetch_row($res))
//    {
//        $j = @mysql_num_fields($res);
//        $k = 0;
//        while ($k < $j)
//        {
//            $field = @mysql_fetch_field($res, $k);
//            echo "$field->name : $i[$k]" . "</br>";
//            $k++;
//        }
//    }
	
	$query = "select * from Users";
	$res = @mysql_query($query);
	
	echo "Users:" . "</br>";
	while ($i = mysql_fetch_row($res))
	{
            $j = @mysql_num_fields($res);
            $k = 0;
            while ($k < $j)
            {
                $field = @mysql_fetch_field($res, $k);
                if ($field->name == "Photo")
                {
                    echo "$field->name :";
                    echo '<p><img src="data:image/png;base64,'
                            . $i[$k]
                            . '" /></p>';
                }
                else
                {
                    echo "$field->name : $i[$k]" . "</br>";
                }
                $k++;
            }
	}
	
//	$query = "select * from Stops";
//	$res = @mysql_query($query);
//	
//	echo "Stops:" . "</br>";
//	while ($i = mysql_fetch_row($res))
//	{
//		echo "$i[0]" . "</br>";
//	}
//	
//	$query = "select * from GPS";
//	$res = @mysql_query($query);
//	
//	echo "GPS:" . "</br>";
//	while ($i = mysql_fetch_row($res))
//	{
//		echo "$i[0]" . "</br>";
//	}
?>