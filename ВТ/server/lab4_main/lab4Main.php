<?php



function match($pattern, &$html, $tag)
{
    $Arr = array();
    if(preg_match_all($pattern, $html, $matches))
    {
        $count = 0;
        foreach ($matches[1] as $key)
        {
            $Arr[] = $key;
            $html = preg_replace('/<' . $tag . '>' . $key . '<\/' . $tag . '>/Ui', '{!'. $tag .'!'.$count.'!!}', $html,1);
            $count++;
        }
        return $Arr;
    }
}

function matchStyles($pattern, &$htm)
{
	$Arr = array();
	if(preg_match_all($pattern, $html, $matches)
	{
		$count = 0;
		foreach ($matches[1] as $key)
		{
			$Arr[] = $key;
		}
	}
	return $Arr;
}



function searchStyleElHtml($pattern, &$html, $color)
{
	
	
}





function Replace($arr, $tag, $color, &$html)
{
    if (!empty($arr))
    {
        for($j = 0; $j < count($arr); $j++)
        {
            $html = str_replace('{!'. $tag . '!'.$j.'!!}', "<". $tag . " style='" . "color: " . $color . ";'>" . $arr[$j]. "</". $tag . ">", $html);
        }
    }
}

$url = "DOP.html";
$html = file_get_contents($url);

echo $html . "<br>";
$pattern_b = '/<b>(.*)<\/b>/Ui';
$arr_b = match($pattern_b, $html, 'b');
$pattern_i = '/<i>(.*)<\/i>/Ui';
$arr_i = match($pattern_i, $html, 'i');
$pattern_u = '/<u>(.*)<\/u>/Ui';
$arr_u = match($pattern_u, $html, 'u');

$pattern_strong = '/<strong>(.*)<\/strong>/Ui';
$arr_strong = match($pattern_strong, $html, 'strong');

$pattern_em = '/<em>(.*)<\/em>/Ui';
$arr_em = match($pattern_em, $html, 'em');

$pattern_underline = '/{(.*)text-decoration: underline;(.*)}/Ui';
$arr_underline_ID = matchStyles($pattern_underline, $html);
$pattern_thick = '/{(.*)text-decoration: ;(.*)}/Ui';
$arr_thick_ID = matchStyles($pattern_thick, $html);
$pattern_oblique = '/{(.*)text-decoration: ;(.*)}/Ui';
$arr_oblique_ID = matchStyles($pattern_oblique, $html);


$pattern_Class_ID_search = '/<(.*)>(.*)(class|id)="\/"<(.*)>/Ui';





if ((!empty($arr_b)) || (!empty($arr_i)) || (!empty($arr_u)) || (!empty($arr_strong)) || (!empty($arr_em)))
{
    Replace($arr_b, 'b', 'red', $html);
    Replace($arr_i,'i','green',$html);
    Replace($arr_u,'u','blue',$html);
    Replace($arr_strong,'strong','red',$html);
    Replace($arr_em,'em','green',$html);
}
echo $html;