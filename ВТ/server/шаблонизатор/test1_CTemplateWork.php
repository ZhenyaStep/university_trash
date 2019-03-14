<?php

class CTemplateWork
{

	private $lng;
	private $cfg;
	private $dv;

	function __construct($lng, $cfg, $dv)
	{
		$this->lng = $lng;
		$this->cfg = $cfg;
		$this->dv = $dv;
	}	

	function getLng() { return  $this->$lng; }

	function getCfg() { return $this->$cfg; }

	function getDv() { return $this->$dv; }

	function setLng($lng) { $this->$lng = $lng; }

	function setCfg($cfg) { $this->$cfg = $cfg; }

	function setDv($dv) { $this->$dv = $dv; }





	function replacerCFG($X)
	{

		if (isset($cfg[$lng][$X[1]])) {
			return $cfg[$lng][$X[1]];
		}
		return "ERROR: " . $X[1] . "NOT FOUND! </br>";
	}

	function replacerDB($X)
	{
		return $X;
	}

	function replacerDV($X)
	{
		if (isset($dv[$X[1]])) {
			return $dv[$X[1]];
		}
		return "ERROR: " . $X[1] . "NOT FOUND! </br>";
	}

	function replacerFile($X)
	{
		if (is_file($X))
		{
			return file_get_contents($X);
		}
		return "ERROR: " . $X[1] . "NOT FOUND! </br>";
	}



	function replacer($document)
	{

		$document = preg_replace_callback("/{CFG=\"(.*)\"}/U", 'replacerCFG', $document);
		$document = preg_replace_callback("/{DV=\"(.*)\"}/U", 'replacerDV', $document);
		$document = preg_replace_callback("/{FILE=\"(.*)\"}/U", 'replacerFile', $document);

		$document = preg_replace_callback("/{DB_TABLE=\"(.*)\" DB_VALUE=\"(.*)\"}/U", 'replacerDB', $document); //?
		return $document;
	}
}	

