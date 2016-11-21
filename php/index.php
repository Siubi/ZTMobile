<?php
    session_start();
    require "utils.php";
?>

<html>
    <head>
        <title>Strona główna</title>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
    </head>
    <body>
        <?php
            loginDiv();
        ?>
        <div>
            <?php
                if (isset($_SESSION['login']))
                {
                    echo '<p><a href="files.php">files</a></p>';
                    echo '<p><a href="create_raport.php">create raport</a></p>';
                }
            ?>
        </div>
    </body>
</html>
