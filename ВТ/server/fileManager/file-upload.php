<?

$errors ="";
$filedirectory="";
include("includeFileManager.php");


if(array_key_exists("dir",$_GET))
{
	$filedirectory = resolveFilename(HOME_PATH.urldecode(str_replace(":","",$_GET["dir"])));
	if(array_key_exists("load",$_POST))
	{
		if(checkFilePath($filedirectory))
		{	
			if(file_exists($filedirectory))
			{
				$good_files = 0;
				foreach ($_FILES["userfile"]["error"] as $key => $error) {
					if ($error == UPLOAD_ERR_OK) {
						$tmp_name = $_FILES["userfile"]["tmp_name"][$key];
						// basename() может спасти от атак на файловую систему;
						// может понадобиться дополнительная проверка/очистка имени файла
						$name = basename($_FILES["userfile"]["name"][$key]);
						if((!file_exists($filedirectory."/$name"))||(array_key_exists("rewrite",$_POST))){
							if(move_uploaded_file($tmp_name, $filedirectory."/$name")){$good_files++;};
						}
						else{
							$path_info = pathinfo($name);
							if(move_uploaded_file($tmp_name, $filedirectory."/".$path_info['filename'].date("_M_j_Y_G_i_s",time()).$key.".".$path_info['extension'])){$good_files++;};
						}							
					}
				}
				echo "Upload $good_files files";
			}
			else
			{
				$errors .="Directory don't exist \"".$filedirectory."\"".PHP_EOL;
			}
		}
		else
		{
			$errors .="You don't have permission to access \"".$filedirectory."\"".PHP_EOL;
		}
	}
}
else
{
	$errors .="No enough param".PHP_EOL;
}
echo $errors;
?>

<form method="post" enctype="multipart/form-data">
  Загрузка в директорию:<p class="load_directory"><?echo $filedirectory?></p>
  Файлы:<br />
  <input name="userfile[]" type="file" /><br />
  <input name="userfile[]" type="file" /><br />
  <input name="userfile[]" type="file" /><br />
  <input name="userfile[]" type="file" /><br />
  <input name="userfile[]" type="file" /><br />
  <input name="userfile[]" type="file" /><br />
  <input name="userfile[]" type="file" /><br />
  <br />
  <p>Перезапись:<input type="checkbox" name="rewrite"></p>
  <input type="submit" value="Отправить" name="load"/>
</form>