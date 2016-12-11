<?php
    session_start();
    require "utils.php";
?>

<html>
    <head>
        <title>Logowanie</title>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
    </head>
    <body>
        <?php
            loginDiv();
        ?>
        <div>
            <h2>Logowanie</h2>
            <h3>
                <?php
                    checkUser();
                ?>
            </h3>
            
            <form role = "form" method = "post">
                <p>Login:
                    <input type = "text" name = "login" required autofocus>
                </p>
                <p>Hasło:
                    <input type = "password" name = "password" required>
                </p>
                <button type = "submit" name = "logging">Zaloguj</button>
            </form>
            <?php
                if (isset($_SESSION['login']))
                {
                    redirectEcho("index.php");
                }
            ?>
        </div>
        <div>
            <a href="index.php">Strona główna</a>
        </div>
    </body>
</html>
