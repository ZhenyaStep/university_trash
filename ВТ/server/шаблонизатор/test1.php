<?php

require_once('test1.cfg');
require_once('test1.dv');
require_once('test1_CTemplateWork.php');

$document = file_get_contents("test1.html");

$lng = 'ru';
$cfg = $config;

switch($lng) {
	case 'ru':
		$dv = $dynamic_vars_rus;
		break;
	default: //en 
		$dv = $dynamic_vars_en;
		break;
}

$TemplateWork = new CTemplateWork($lng, $cfg, $dv);
echo $document = $TemplateWork->replacer($document);












