<?
	
	$errors ="";
	$filedirectory="";
	include("includeFileManager.php");


	if(array_key_exists("file",$_GET))
	{
		$filedirectory = iconv("UTF-8","cp1251",resolveFilename(HOME_PATH.urldecode(str_replace(":","",$_GET["file"]))));
		if (!checkFilePath($filedirectory)){ $errors .= "You don't have permission to access \"".$filedirectory."\"".PHP_EOL;}
		elseif (!file_exists($filedirectory)){ $errors .= "This file doen't exist \"".$filedirectory."\"".PHP_EOL;}
		else
		{
			file_force_download($filedirectory);
		}

	}
	else
	{
		$errors .="No enough param".PHP_EOL;
	}
	
	if($errors != ""){echo $errors;}
	
	
	
	function file_force_download($file) {
	  if (file_exists($file)) {
		// сбрасываем буфер вывода PHP, чтобы избежать переполнения памяти выделенной под скрипт
		// если этого не сделать файл будет читаться в память полностью!
		if (ob_get_level()) {
		  ob_end_clean();
		}
		// заставляем браузер показать окно сохранения файла
		header('Content-Description: File Transfer');
		header('Content-Type: application/octet-stream');
		header('Content-Disposition: attachment; filename=' . basename($file));
		header('Content-Transfer-Encoding: binary');
		header('Expires: 0');
		header('Cache-Control: must-revalidate');
		header('Pragma: public');
		header('Content-Length: ' . filesize($file));
		// читаем файл и отправляем его пользователю
		readfile($file);
		exit;
	  }
	}
?>