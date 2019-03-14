<?php

$html = '
    <b>erebbtКрасный текстetbb</b>, 
    <i>Зеленый текст</i>, 
    <u>Синий текст</u>, 
    <b>Красный текст 2</b>, 
    <i>2</i>, 
    <u>Синий текст 2</u>, 
    <b>Красный текст 3</b>, 
    <i>Зеленый текст 3</i>, 
    <u>Синий текст 3</u>';
 
echo $html;
echo '<hr />';
$html2 = $html;
 
$b = array();
$pattern_b = '/<b>(.*)<\/b>/Ui';
$i = array();
$pattern_i = '/<i>(.*)<\/i>/Ui';
$u = array();
$pattern_u = '/<u>(.*)<\/u>/Ui';

$pattern_underline = '/#(.*){(.*)text-decoration: underline;(.*)}/Ui';
$pattern_thick = '/#(.*){(.*)text-decoration: underline;(.*)}/Ui';
$pattern_oblique = '/#(.*){(.*)text-decoration: underline;(.*)}/Ui';



if(preg_match_all($pattern_underline, $html2, $matches))
{
    $bcount = 0;

    foreach($matches[1] as $m)
    {
        $b[] = $m;

        $html2 = preg_replace('/<b>'.$m.'<\/b>/Ui', '{!b!'.$bcount.'!!}', $html2, 1);

        $bcount++;
    }
}



if(preg_match_all($pattern_b, $html2, $matches))
{
    $bcount = 0;
 
    foreach($matches[1] as $m)
    {
        $b[] = $m;
 
        $html2 = preg_replace('/<b>'.$m.'<\/b>/Ui', '{!b!'.$bcount.'!!}', $html2, 1);
 
        $bcount++;
    }
}
 
if(preg_match_all($pattern_i, $html2, $matches))
{
    $icount = 0;
 
    foreach($matches[1] as $m)
    {
        $i[] = $m;
 
        $html2 = preg_replace('/<i>'.$m.'<\/i>/Ui', '{!i!'.$icount.'!!}', $html2, 1);
 
        $icount++;
    }
}
 
if(preg_match_all($pattern_u, $html2, $matches))
{
    $ucount = 0;
 
    foreach($matches[1] as $m)
    {
        $u[] = $m;
 
        $html2 = preg_replace('/<u>'.$m.'<\/u>/Ui', '{!u!'.$ucount.'!!}', $html2, 1);
 
        $ucount++;
    }
}
 
if(!empty($b) || !empty($i) || !empty($u))
{
    if(!empty($b))
    {
        for($j = 0; $j < count($b); $j++)
        {
            $html2 = str_replace("{!b!".$j."!!}", "<b style=\"color: red;\">".$b[$j]."</b>", $html2);
        }
    }
 
    if(!empty($i))
    {
        for($j = 0; $j < count($i); $j++)
        {
            $html2 = str_replace("{!i!".$j."!!}", "<i style=\"color: green;\">".$i[$j]."</i>", $html2);
        }
    }
 
    if(!empty($u))
    {
        for($j = 0; $j < count($u); $j++)
        {
            $html2 = str_replace("{!u!".$j."!!}", "<u style=\"color: blue;\">".$u[$j]."</u>", $html2);
        }
    }
 
    $html2 = preg_replace('/{!(b|u|i)![0-9]+!!}/Ui', "", $html2);
}
 
echo $html2;
