<?php
/*
Plugin Name: APML support for WordPress 2.0.x - 2.2.x
Plugin URI: http://notizblog.org/projects/apml-for-wordpress/
Description: This plugin creates an APML Feed using the categories.
Version: 1.0
Author: Matthias Pfefferle
Author URI: http://notizblog.org/
*/

if (empty($wp)) {
	require_once('./wp-config.php');
	wp();
}

$date = date('Y-m-d\Th:i:s');
$url = get_bloginfo('url');
$url = str_replace('https://', '', $url);
$url = str_replace('http://', '', $url);

$cat_max = $wpdb->get_var("SELECT MAX(category_count) FROM $wpdb->categories");
$categories = $wpdb->get_results("SELECT * FROM $wpdb->categories");

//print_r($categories);

header('Content-Type: text/xml; charset=' . get_option('blog_charset'), true);

echo ('<?xml version="1.0"?>')

?>
<APML xmlns="http://www.apml.org/apml-0.6" version="0.6" ><Head>   <Title>Taxonomy APML for <?php echo get_bloginfo('name', 'display') ?></Title>   <Generator>wordpress/<?php bloginfo_rss('version') ?></Generator>   <DateCreated><?php echo $date; ?></DateCreated></Head><Body defaultprofile="taxonomy">
    <Profile name="categories">        <ImplicitData>            <Concepts>
<?php foreach ($categories as $cat) : ?>
                <Concept key="<?php echo $cat->cat_name; ?>" value="<?php echo (($cat->category_count*100)/$cat_max)/100; ?>" from="<?php echo $url; ?>" updated="<?php echo $date; ?>"/>
<?php endforeach; ?>
            </Concepts>        </ImplicitData>    </Profile></Body></APML>