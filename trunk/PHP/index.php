<html>
<head>
<title>APML Parser Example</title>
</head>
<body>
<?php
require_once('apml_parser.php');

// define url of .apml file
$url = 'https://apml.engagd.com/apml/ash.is.engagd.com';

// define new parser class
$parser = new APML_Parser();
$apml_array = $parser->getAPMLConcepts($url);

// loop through and echo each apml concept
for($i=0; $i<sizeof($apml_array); $i++) {
	echo ucwords($apml_array[$i]['concept_key']) . '<br />' .
		$apml_array[$i]['value'] . '<br />' .
		$apml_array[$i]['updated'] . '<br />' .
		$apml_array[$i]['from'] . '<br /><br />';
}
?>
</body>
</html>