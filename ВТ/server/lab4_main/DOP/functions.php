<?php
    define('COUNT_RECURSION', 20);
    header('Content-Type: text/html; charset= utf-8');

    function getConfig($config_path)
    {
        $read = file($config_path);
        if ($read != "")
        {
            $resulrArr = array();
            foreach ($read as $key)
            {
                $explodedstr = explode('=', $key, 2);
                if (count($explodedstr) == 2)
                {
                    $resulrArr[trim($explodedstr[0])] = $explodedstr[1];
                }
            }
        }
        else
        {
            $resulrArr = "incorrect filename.";
        }
        return $resulrArr;
    }

function ReplaserFile($fileName){
    $file=file($fileName);
    if ($file=="")
    {
        $file="Incorrect Name of file";
    }
    return $file;
}


    function pregReplaserConfig()
    {

    }
    function pregReplaserVar()
    {

    }

    function pregReplaserIf()
    {

    }

    function pregReplaserDB()
    {

    }

    function Replacing($FILE, $vars_array, $config_path, $db_table, $mysqli)
    {
        $content = file_get_contents('main.html');
        $config = getConfig($config_path);
        $resultsql='';
        if($mysqli == null)
        {
            $resultsql='Incorrect DataBase';
        }

        if($resultMySQL = $mysqli->query("SELECT * FROM users"))
        {
            if ($row = $resultMySQL->fetch_array(MYSQLI_ASSOC)) {
                echo $row;
            }
            $resultMySQL->free();
        } else{$resultsql = "incorrect DB";}
        echo $resultsql;

        $ArrayTag = array(
          array('/\{FILE\s*=\s*\"([^\"]+)\"\}/', 'ReplaserFile'),
            array('/\{CONFIG\s*=\s*\"([a-zA-Z_\x7f-\xff][a-zA-Z0-9_\x7f-\xff]*)\"\}/',
                $pregReplaserConfig),
            array('/\{VAR\s*=\s*\"([a-zA-Z_\x7f-\xff][a-zA-Z0-9_\x7f-\xff]*)\"\}/',
                $pregReplaserVar),
            array('/\{DB\s*=\s*\"([a-zA-Z_\x7f-\xff][a-zA-Z0-9_\x7f-\xff]*)\"\}/',
                $pregReplaserDB),
          array('', $pregReplaserIf)
        );

        preg_match_all('/\{VAR\s*=\s*\"([a-zA-Z_\x7f-\xff][a-zA-Z0-9_\x7f-\xff]*)\"\}/', $content, $out, PREG_PATTERN_ORDER);
        for ($i=0; $i<count($out[0]); $i++)
        {
            $tmp = $out[1]
        }



        for ($i = 0; $i < COUNT_RECURSION; $i++)
        {
            for ($j=0; $j<count($ArrayTag); $j++)
            {
                $str = preg_replace_callback($ArrayTag[$j][0], $ArrayTag[$j][1], $str);
            }
        }

        return $content;
    }