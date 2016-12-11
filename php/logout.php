<?php
    require "utils.php";
    session_start();

    unset($_SESSION["login"]);
    unset($_SESSION["password"]);
    $_SESSION['valid'] = false;
    
    redirectEcho("index.php");
?>
