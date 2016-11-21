<?php
        $server = "s12.hekko.net.pl";
	$db = "ztmobile_0";
	$port = "3306";
        $admin = "ztmobile_0";
        $adminPass = "admin123";
	
	function connect($login, $pass)
	{
		global $server, $db, $port;
		$connection = @mysql_connect($server. ":" . $port, $login, $pass)
			or die("Brak połączenia!"."</br>");
		//echo "Połączono z serverem $server"."</br>";
		@mysql_select_db($db)
			or die("Brak bazy danych!"."</br>");
		//echo "Wybrano bazę $db"."</br>";
	}
		
	connect($admin, $adminPass);
        
        function findCurrentUserInDb($userLogin)
        {
            $query = "select * from Users where Login='" . $userLogin . "'";
            $res = @mysql_query($query);
            $userAssoc = array();
            $userAssoc['login'] = 'undefined';
            if ($resAssoc = @mysql_fetch_assoc($res))
            {
                foreach ($resAssoc as $key => $value)
                {
                    $userAssoc[strtolower($key)] = $resAssoc[$key];
                }
            }
            return $userAssoc;
        }
        
        function checkUser()
        {
            global $admin, $adminPass;
            
            if (isset($_POST['logging'])
                    && !empty($_POST['login'])
                    && !empty($_POST['password']))
            {
                $userAssoc = findCurrentUserInDb($_POST['login']);
                if ($userAssoc['login'] != 'undefined'
                       && $_POST['login'] == $userAssoc['login']
                       && strtoupper(md5($_POST['password'])) == $userAssoc['password'])
                {
                    $_SESSION['valid'] = true;
                    $_SESSION['timeout'] = time();
                    $_SESSION['login'] = $_POST['login'];

                    echo "Zalogowano jako: " . $_SESSION['login'];
                }else 
                {
                    echo "Niepoprawne dane logowania!";
                    $_SESSION['valid'] = false;
                }
            }
        }
        
        function getPathFiles($line, $dateFrom, $dateTo)
        {
            $query = "select File from RouteTrackFiles where "
                    . "Bus = '" . $line . "'"
                    . " and " . "Date >= '" . $dateFrom . "'"
                    . " and " . "Date <= '" . $dateTo . "'";
            $res = @mysql_query($query);
            $k = 0;
            while ($i = @mysql_fetch_row($res))
            {
                echo $k++ . " " /*. $i[0]*/ . "</br>" . "</br>";
            }
            if ($k == 0)
            {
                echo "Brak plików!" . "</br>";
            }
        }    
?>