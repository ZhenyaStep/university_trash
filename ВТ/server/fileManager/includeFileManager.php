<?
	define("HOME_PATH",resolveFilename($_SERVER['DOCUMENT_ROOT']."/../../"));
	function checkFilePath($filepath){
		return strrpos(resolveFilename($filepath),substr(HOME_PATH,0,-1)) === 0;
	}
	
	function resolveFilename($filename)
	{
		$filename = str_replace('\\', '/', $filename);
		$filename = str_replace('//', '/', $filename);
		$parts = explode('/', $filename);
		$out = array();
		foreach ($parts as $part){
			if ($part == '.') continue;
			if ($part == '..') {
				array_pop($out);
				continue;
			}
			$out[] = $part;
		}
		return implode('\\', $out);
	}
?>