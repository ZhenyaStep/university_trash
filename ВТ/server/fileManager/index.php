<?
	
	include("includeFileManager.php");

	$shablonTable = file_get_contents("table_sbl.html");	
	
	$error = "";
	$confimWindow = "";
	if(array_key_exists("dir",$_GET)){
		$filepath = resolveFilename(HOME_PATH.urldecode(str_replace(":","",$_GET["dir"])));
		if (!checkFilePath($filepath)){ $error .= "You don't have permission to access \"".$filepath."\"".PHP_EOL; $filepath = HOME_PATH;}
		if (!file_exists($filepath)){ $error .= "This directory doen't exist \"".$filepath."\"".PHP_EOL; $filepath = HOME_PATH;}
	}
	else{
		$filepath = HOME_PATH;
	}
	
	$filePost = getArrayFromPost($_POST,$filepath);
	
	$error .= Handler($filePost,$filepath);	
	
	$shablonControlElem = getMainControlElem(file_get_contents("control_elem_main.html"),urldecode(str_replace(HOME_PATH,"",str_replace('/',"\\",iconv("cp1251", "UTF-8" , $filepath)))));
	echo htmlCarrentDirectory($filepath,$filePost,$shablonControlElem,$confimWindow,$error);
	
	
	
	
	
	
	
	function Handler(&$filePost,&$nowPath){
		global $confimWindow;
		if (array_key_exists("del",$_POST)){
			if(isConfim()){
				deleteArrayFile($filePost);
				$filePost = array();
			}
			else if (!array_key_exists("cancel",$_POST)){
				if(count($filePost)>0){
					$text = "Delete file:".PHP_EOL;
					foreach($filePost as $elem){
						$text .= $elem.PHP_EOL; 
					}
					$confimWindow = getConfimWindow($text,createConfimArray($filePost),"del");
				}
			}
		}
		elseif (array_key_exists("copy",$_POST)){
			if(isConfim()){
				if(array_key_exists("data",$_POST)){
					$path = $_POST["data"];
					if(checkFilePath($path))
					{	
						if(file_exists($path))
						{
							foreach ($filePost as $filename){
								if($filename !=""){
									my_copy_all($filename,resolveFilename($path."\\".basename($filename)),array_key_exists("rewrite",$_POST));
								}
							}	
							$filePost = array();
						}
						else {
							$confimWindow = getInputWindow("Input path",$path,createConfimArray($filePost),"copy");
							return "Directory don't exist \"".$path."\"".PHP_EOL;
						}
					}
					else {
						$confimWindow = getInputWindow("Input path",$path,createConfimArray($filePost),"copy");
						return "You don't have permission to access \"".$path."\"".PHP_EOL;
					}
				}
				else 
				{
					$confimWindow = getInputWindow("Input path","",createConfimArray($filePost),"copy");
					return "No path!".PHP_EOL;
				}
			}
			else if (!array_key_exists("cancel",$_POST)){
				if(count($filePost)>0){
					$confimWindow = getInputWindow("Input path",$nowPath,createConfimArray($filePost),"copy");
				}
			}
		}
		elseif (array_key_exists("move",$_POST)){
			if(isConfim()){
				if(array_key_exists("data",$_POST)){
					$path = $_POST["data"];
					if(checkFilePath($path))
					{	
						if(file_exists($path))
						{
							foreach ($filePost as $filename){
								if($filename !=""){
									my_rename_all($filename,resolveFilename($path."\\".basename($filename)),array_key_exists("rewrite",$_POST));
								}
							}	
							$filePost = array();
						}
						else {
							$confimWindow = getInputWindow("Input path",$path,createConfimArray($filePost),"move");
							return "Directory don't exist \"".$path."\"".PHP_EOL;
						}
					}
					else {
						$confimWindow = getInputWindow("Input path",$path,createConfimArray($filePost),"move");
						return "You don't have permission to access \"".$path."\"".PHP_EOL;
					}
				}
				else 
				{
					$confimWindow = getInputWindow("Input path","",createConfimArray($filePost),"move");
					return "No path!".PHP_EOL;
				}
			}
			else if (!array_key_exists("cancel",$_POST)){
				if(count($filePost)>0){
					$confimWindow = getInputWindow("Input path",$nowPath,createConfimArray($filePost),"move");
				}
			}
		}
		elseif (array_key_exists("rename",$_POST)){
			if(count($filePost) == 1)
			{
				if(isConfim()){
					if(array_key_exists("data",$_POST))
					{
						$name = $_POST["data"];
						if(checkFilePath($nowPath."\\".$name))
						{	
							if(!file_exists($nowPath."\\".$name)||array_key_exists("rewrite",$_POST))
							{
								$filename = $filePost[0];
								if($filename !=""){
									my_rename_all($filename,$nowPath."\\".$name,array_key_exists("rewrite",$_POST));
								}	
								$filePost = array();
							}
							else{
								$confimWindow = getInputWindow("Input name",$name,createConfimArray($filePost),"rename");
								return "This file already exist \"".$name."\"".PHP_EOL;
							}
						}
						else {
							$confimWindow = getInputWindow("Input name",$name,createConfimArray($filePost),"rename");
							return "You don't have permission to access \"".$nowPath."\\".$name."\"".PHP_EOL;
						}
					}
					else 
					{
						$confimWindow = getInputWindow("Input name","",createConfimArray($filePost),"rename");
						return "No name!".PHP_EOL;
					}
				}
				else if (!array_key_exists("cancel",$_POST)){
					if(count($filePost)>0){
						$confimWindow = getInputWindow("Input name","",createConfimArray($filePost),"rename");
					}
				}
			}
			else
			{
				return "Select only one file!";
			}
		}
		elseif (array_key_exists("create_dir",$_POST)){
			if(isConfim())
			{
				if(array_key_exists("data",$_POST))
				{
					$name = $_POST["data"];
					if(checkFilePath($nowPath."\\".$name))
					{	
						if(!file_exists($nowPath."\\".$name))
						{
							mkdir($nowPath."\\".$name);
						}
						else{
							$confimWindow = getInputWindow("Input name",$name,createConfimArray($filePost),"create_dir");
							return "This directory already exist \"".$name."\"".PHP_EOL;
						}
					}
					else {
						$confimWindow = getInputWindow("Input name",$name,createConfimArray($filePost),"create_dir");
						return "You don't have permission to access \"".$nowPath."\\".$name."\"".PHP_EOL;
					}
				}
				else 
				{
					$confimWindow = getInputWindow("Input name","",createConfimArray($filePost),"create_dir");
					return "No name!".PHP_EOL;
				}
			}
			else if (!array_key_exists("cancel",$_POST)){
				$confimWindow = getInputWindow("Input name","",createConfimArray($filePost),"create_dir");
			}
		}
	}
	
	function my_rename_all($from, $to, $rewrite = true) {
		if(file_exists($to))
		{
			if (is_dir($from)) {
				$d = dir($from);
				while (false !== ($entry = $d->read())) {
					if ($entry == "." || $entry == "..") {continue;}
					my_rename_all("$from/$entry", "$to/$entry", $rewrite);
				}
				$d->close();
				if(count(scandir($from)) == 2){unlink($to);}
			} 
			else 
			{
				if ($rewrite)
				{
					unlink($to);
					rename($from, $to);
				};
			}
		}
		else{
			rename($from, $to);
		}
	}
	
	function my_copy_all($from, $to, $rewrite = true) {
		if (is_dir($from)) {
			@mkdir($to);
			$d = dir($from);
			while (false !== ($entry = $d->read())) {
				if ($entry == "." || $entry == "..") {continue;}
				my_copy_all("$from/$entry", "$to/$entry", $rewrite);
			}
			$d->close();
		} 
		else 
		{
			if (!file_exists($to) || $rewrite){copy($from, $to);};
		}
	}
	
	function getMainControlElem($shablon,$filepath){
		return str_replace("{DIR_LINK}",$filepath,$shablon);
	}
	
	function getConfimWindow($text, $elem,$action){
		$result = file_get_contents("confim_window.html");
		$result = str_replace("{CONFIM_TEXT}",$text,$result);
		$result = str_replace("{CONFIM_ELEM}",$elem,$result);
		$result = str_replace("{CONFIM_ACTION}",$action,$result);
		return $result;
	}
	
	function getInputWindow($text,$data,$elem,$action){
		$result = file_get_contents("window_for_input.html");
		$result = str_replace("{TEXT}",$text,$result);
		$result = str_replace("{DATA}",$data,$result);
		$result = str_replace("{CONFIM_ELEM}",$elem,$result);
		$result = str_replace("{CONFIM_ACTION}",$action,$result);
		return $result;
	}
	
	function isConfim(){
		return array_key_exists("confim",$_POST);
	}
	
	function getArrayFromPost(&$PostArrray,$filepath){
		$result = array();
		$files = scandir(iconv("UTF-8","cp1251",$filepath));
		foreach(array_keys($PostArrray) as &$num){
			if ((is_numeric($num))&&($num>1)&&($num<count($files)&&file_exists($filepath."/".$files[$num]))){
				if(checkFilePath($filepath."\\".$files[$num])){
					array_push($result,$filepath."\\".$files[$num]);
				}
				else { $error .= "You don't have permission to access \"".$filepath."\"".PHP_EOL;}
			}
		}
		return $result;
	}
	
	function getConfimArray(){
		$result=array();
		if(array_key_exists("confim_data",$_POST)){
			$result=explode(";",$_POST["confim_data"]);
			foreach($result as &$elem){
				if(checkFilePath($elem)){$elem=urldecode($elem);}
				else {$elem = "";}
			}
			array_pop($result);
		}
		return $result;
	}
	
	function createConfimArray($arrayConfim){
		$result="";
		foreach($arrayConfim as &$elem){
			$result .= urlencode($elem).";";
		}
		return $result;
	}
	
	function htmlCarrentDirectory($filepath,&$filePost,&$controlElem,&$confimWindow,&$error){
		$shablonFileMananger = file_get_contents("fileManager_sbl.html");
		$result = $shablonFileMananger;
		if($error != ""){
			$shablonError = str_replace("{ERROR_MESSAGE}",$error,file_get_contents("error_sbl.html"));
		}
		else{$shablonError = "";}
		$result = str_replace("{PATH}",$filepath,$result);
		$result = str_replace("{ERRORS}",$shablonError,$result);
		$result = str_replace("{TABLE_DIR}",tableCarrentDirectory($filepath,$filePost),$result);
		$result = str_replace("{CONTROL_ELEM}",$controlElem,$result);
		$result = str_replace("{CONFIM_WINDOW}",$confimWindow,$result);
		return $result;
	}
	
	function tableCarrentDirectory($filename,&$ChechedFile){	
		$files = scandir(iconv("UTF-8","cp1251",$filename));
		$result = "";
		for($i=1;$i<count($files);$i++)
		{
			$isCheck = in_array($filename."\\".$files[$i],$ChechedFile);
				
			$result .= getFileTable($filename."\\".$files[$i],$i,$isCheck);
		}
		
		return $result;	
	}
	
	function deleteArrayFile($arrayFile){
		foreach($arrayFile as $filename){
			if(!file_exists($filename)){continue;}
			if(is_dir($filename)){removeDirectory($filename);}
			else {unlink($filename);}
		}
	}
	
	function removeDirectory($dir) {
		if ($objs = glob($dir."/*")) {
		   foreach($objs as $filename) {
		   if(!file_exists($filename)){continue;}
			 is_dir($filename) ? removeDirectory($filename) : unlink($filename);
		   }
		}
		rmdir($dir);
	}
	
	function getFileTable($filename,$inputName,$isCheck){
		if(!file_exists($filename)){return -1;}
		global $shablonTable;

		if ($isCheck){$strCheck = "checked";}
			else {$strCheck = "";}
		
		$TagArray = array(
			array("{FILE_NAME}",iconv("cp1251", "UTF-8" , basename($filename))),
			array("{FILE_ICON}",get_file_icon($filename)),
			array("{FILE_SIZE}",get_filesize($filename)),
			array("{FILE_DATA}",date("M j, Y G:i",filemtime($filename))),
			array("{FILE_TYPE}",_mime_content_type($filename)),
			array("{FILE_INPUT_NAME}",$inputName),
			array("{FILE_HREF}",get_file_href($filename)),
			array("{IS_CHECK}",$strCheck)
			);
		return shablonReplaser($shablonTable,$TagArray);
	}
	
	function shablonReplaser($changeStr, &$TagArray){
		for($i=0;$i<count($TagArray);$i++)
		{
			$changeStr = str_replace($TagArray[$i][0],$TagArray[$i][1],$changeStr);
		}
		return $changeStr;
	}
	
	function get_filesize($file)
	{
	  if(!file_exists($file)) return -1;

	  $filesize = filesize($file);

		if($filesize > 1024){
			$filesize = ($filesize/1024);
			if($filesize > 1024){
				$filesize = ($filesize/1024);
				if($filesize > 1024) {
					$filesize = ($filesize/1024);
					$filesize = round($filesize, 1);
					return $filesize." GB";       
				} 
				else 
				{
					$filesize = round($filesize, 1);
					return $filesize." MB";   
				}       
			} 
			else 
			{
				$filesize = round($filesize, 1);
				return $filesize." KB";   
			}  
		} 
		else 
		{
			$filesize = round($filesize, 1);
			return $filesize." bytes";   
		}
	}
	
	function get_file_href($filename){
		
		if(is_dir($filename)){
			$href = basename( __FILE__)."?dir=".urldecode(str_replace(HOME_PATH,"",str_replace('/',"\\",iconv("cp1251", "UTF-8" , $filename))));
		}
		else{
			$href = "download.php?file=".urldecode(str_replace(HOME_PATH,"",str_replace('/',"\\",iconv("cp1251", "UTF-8" , $filename))));
		}
		return $href;
	}
	
	function get_file_icon($filename){
		$icon = "/mngIcons/".str_replace('/','-',_mime_content_type($filename).".png");
		if(!file_exists(".".$icon)){
			if(strrpos(mime_content_type($filename),'audio') !== false){$icon = "/mngIcons/audio-generic.png";}
			else if(strrpos(mime_content_type($filename),'video') !== false){$icon = "/mngIcons/video-generic.png";}
			else if(strrpos(mime_content_type($filename),'image') !== false){$icon = "/mngIcons/image-x-generic.png";}
			else if(strrpos(mime_content_type($filename),'package') !== false){$icon = "/mngIcons/package-x-generic.png";}
			else {$icon = "/mngIcons/file-generic.png";}
		}
		return $icon;
	}
	
	function _mime_content_type($filename) {

        $mime_types = array(
            'htm' => 'text/html',
            'html' => 'text/html',
            'css' => 'text/css',
			'xml' => 'text/xml',
			'log' => 'text/x-log',
            'js' => 'application/javascript',
            'json' => 'application/json',
            'xml' => 'application/xml',
            'sql' => 'text/x-sql',
            'doc' => 'application/msword',
			'docx' => 'application/msword',
            'xls' => 'application/vnd.ms-excel',
            'ppt' => 'application/vnd.ms-powerpoint',
			'exe' => 'application-x-executable'
        );
		$exp = explode('.',$filename);
        $ext = strtolower(array_pop($exp));
        if (array_key_exists($ext, $mime_types)) {
            return $mime_types[$ext];
        }
        elseif (function_exists('mime_content_type')) {
            return mime_content_type($filename);
        }
        else {
            return 'application/octet-stream';
        }
	}

?>