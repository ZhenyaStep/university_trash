<?php

include 'ChromePhp.php';
ChromePhp::log('hello world');
ChromePhp::log($_SERVER);
 
// использование меток
foreach ($_SERVER as $key => $value) {
    ChromePhp::log($key, $value);
}
 
// предупреждения и ошибки
ChromePhp::warn('this is a warning');
ChromePhp::error('this is an error');