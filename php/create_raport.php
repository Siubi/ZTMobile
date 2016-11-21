<?php
    session_start();
    require "utils.php";
?>

<html>
    <head>
        <title>Stwórz raport</title>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
    </head>
    <body>
        <?php
            loginDiv();
        ?>
        <div>
            <h2>Stwórz raport</h2>
            
            <form role = "form" method = "post">
                <p>Linia:
                    <input type="text" name="line" required autofocus>
                </p>
                <p>Od:
                    <input type="date" name="dateFrom" value="<?php
                        echo $initialDate;
                        ?>" placeholder="yyyy-mm-dd" required>
                </p>
                <p>Do:
                    <input type="date" name = "dateTo" value="<?php
                        echo date("Y-m-d");
                    ?>" placeholder="yyyy-mm-dd" required>
                </p>
                <button type="submit" name="raportParams">Generate</button>
            </form>
        </div>
        <div>
            <a href="index.php">Strona główna</a>
        </div>
        <div>
            <?php
                if (isset($_POST['raportParams'])
                        && !empty($_POST['line'])
                        && !empty($_POST['dateFrom'])
                        && !empty($_POST['dateTo']))
                {
                    echo "Line: " . $_POST['line'] . "</br>";
                    getPathFiles($_POST['line'], $_POST['dateFrom'], $_POST['dateTo']);
                };
                $_POST = array();
            ?>
        </div>
    </body>
</html>
