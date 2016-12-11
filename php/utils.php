<?php
    require "db.php";
    
    $initialDate = "2016-10-01";
    
    function redirectEcho($address)
    {
        echo '<script type="text/javascript">'
                                . 'window.location = "'
                                . $address
                                .'"'
                                . '</script>';
    }
    
    function loginDiv()
    {
        echo '<div id="loginDiv" style="border: thin solid black">';
            echo "<p>";
                if (isset($_SESSION['login']))
                {
                    $userAssoc = findCurrentUserInDb($_SESSION['login']);
                    echo '<img src="data:image/png;base64,'
                        . $userAssoc["photo"]
                        . '" style="max-width: 200px"/>';
                }
                else
                {
                   echo '<a href="login.php">niezalogowany</a>';
                }
                if (isset($_SESSION['login']))
                {
                    echo '</br><a href="logout.php">Wyloguj</a>';
                }
            echo "</p>";
        echo "</div>";
    }
?>