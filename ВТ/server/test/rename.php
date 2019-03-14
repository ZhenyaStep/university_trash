<?php

$F = "Parent Directory";
iconv("UTF-8","Windows-1251", $F);

echo scandir($F);