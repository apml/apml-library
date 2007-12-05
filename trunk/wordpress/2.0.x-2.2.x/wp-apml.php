<?php
/*
Plugin Name: APML support for WordPress 2.0.x - 2.2.x
Plugin URI: http://notizblog.org/projects/apml-for-wordpress/
Description: This plugin creates an APML Feed using the categories.
Version: 1.2
Author: Matthias Pfefferle
Author URI: http://notizblog.org/
*/

if (empty($wp)) {
  require_once('./wp-config.php');
  wp();
}

$simpletags = $table_prefix . "stp_tags";

$date = date('Y-m-d\Th:i:s');
$url = get_bloginfo('url');
$url = str_replace('https://', '', $url);
$url = str_replace('http://', '', $url);

// check UTW Plugin
if (function_exists('UTW_HasTags')) :
  $tag_max = UltimateTagWarriorCore::GetMostPopularTagCount($date_sensitive);
  $tags = $wpdb->get_results("SELECT tag as name, COUNT(*) count FROM $tabletags t inner join $warriortags2tag p2t on t.tag_id = p2t.tag_id GROUP BY tag");
// check SimpleTagging Plugin
elseif (function_exists('STP_Tagcloud')) :
  $tag_max = $wpdb->get_var("SELECT count(tag_name) FROM $simpletags GROUP BY tag_name");
  $tags = $wpdb->get_results("SELECT tag_name as name, COUNT(*) count FROM $simpletags GROUP BY tag_name");
endif;

$cat_max = $wpdb->get_var("SELECT MAX(category_count) FROM $wpdb->categories");
$categories = $wpdb->get_results("SELECT * FROM $wpdb->categories");

header('Content-Type: text/xml; charset=' . get_option('blog_charset'), true);

echo ('<?xml version="1.0"?>')

?>
<APML xmlns="http://www.apml.org/apml-0.6" version="0.6" >
<Head>
   <Title>Taxonomy APML for <?php echo get_bloginfo('name', 'display') ?></Title>
   <Generator>wordpress/<?php bloginfo_rss('version') ?></Generator>
   <DateCreated><?php echo $date; ?></DateCreated>
</Head>
<Body defaultprofile="taxonomy">
<?php if ($tags): ?>
    <Profile name="tags">
        <ImplicitData>
            <Concepts>
<?php foreach ($tags as $tag) : ?>
                <Concept key="<?php echo $tag->name; ?>" value="<?php echo (($tag->count*100)/$tag_max)/100; ?>" from="<?php echo $url; ?>" updated="<?php echo $date; ?>"/>
<?php endforeach; ?>
            </Concepts>
        </ImplicitData>
    </Profile>
<?php endif; ?>
    <Profile name="categories">
        <ImplicitData>
            <Concepts>
<?php foreach ($categories as $cat) : ?>
                <Concept key="<?php echo $cat->cat_name; ?>" value="<?php echo (($cat->category_count*100)/$cat_max)/100; ?>" from="<?php echo $url; ?>" updated="<?php echo $date; ?>"/>
<?php endforeach; ?>
            </Concepts>
        </ImplicitData>
    </Profile>
</Body>
</APML>
