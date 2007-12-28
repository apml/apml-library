<?php
/*
Plugin Name: APML support for WordPress
Plugin URI: http://notizblog.org/projects/apml-for-wordpress/
Description: This plugin creates an APML Feed using the tags and categories.
Version: 2.3
Author: Matthias Pfefferle
Author URI: http://notizblog.org/
*/

// include the wordpress sources like tags, categories and links
require_once('libs/sources.class.php');

// register
if (isset($wp_version)) {
  add_filter('query_vars', array('APML', 'query_vars'));
  add_action('parse_query', array('APML', 'apml_xml'));
  add_action('init', array('APML', 'flush_rewrite_rules'));
  add_filter('generate_rewrite_rules', array('APML', 'rewrite_rules'));

  add_action('wp_head', array('APML', 'insert_meta_tags'), 5);
}

// apml class
class APML {
  function flush_rewrite_rules() {
    global $wp_rewrite;
    $wp_rewrite->flush_rules();
  }

  function rewrite_rules($wp_rewrite) {
    $new_rules = array(
      'apml$' => 'index.php?apml=apml',
      'apml/(.+)' => 'index.php?apml=apml',
      'wp-apml.php$' => 'index.php?apml=apml'
    );
    $wp_rewrite->rules = $new_rules + $wp_rewrite->rules;
  }

  /**
   * Add 'apml' as a valid query variables.
   **/
  function query_vars($vars) {
    $vars[] = 'apml';

    return $vars;
  }

  /**
   * Print APML document if 'apml' query variable is present
   **/
  function apml_xml() {
    global $wp_query;
    if( isset( $wp_query->query_vars['apml'] )) {
      APML::printAPML();
    }
  }

  function insert_meta_tags() {
    global $wp_rewrite;

    echo '<link rel="meta" type="text+xml" title="APML" href="'.get_option('home').($wp_rewrite->using_mod_rewrite_permalinks() ? '/apml/' : '/index.php?apml=apml').'" />' . "\n";
  }

  function printAPML() {
    global $wp_version;

    $date = date('Y-m-d\Th:i:s');
    $url = get_bloginfo('url');
    $url = str_replace('https://', '', $url);
    $url = str_replace('http://', '', $url);

    $tags = WordPressSources::getTags();
    $tag_max = WordPressSources::getMaxTag();

    $categories = WordPressSources::getCategories();
    $cat_max = WordPressSources::getMaxCategory();
    
    $links = WordPressSources::getLinks();
    
    header('Content-Type: text/xml; charset=' . get_option('blog_charset'), true);
    echo '<?xml version="1.0"?>';
?>
<APML xmlns="http://www.apml.org/apml-0.6" version="0.6" >
<Head>
   <Title>Taxonomy APML for <?php echo get_bloginfo('name', 'display') ?></Title>
   <Generator>wordpress/<?php echo $wp_version ?> and wp-apml/2.3</Generator>
   <DateCreated><?php echo $date; ?></DateCreated>
</Head>
<Body defaultprofile="tags">
    <Profile name="tags">
        <ImplicitData>
            <Concepts>
<?php if (!empty($tags)) { ?>
<?php foreach ($tags as $tag) { ?>
                <Concept key="<?php echo $tag->name; ?>" value="<?php echo (($tag->count*100)/$tag_max)/100; ?>" from="<?php echo $url; ?>" updated="<?php echo $date; ?>"/>
<?php } ?>
<?php } ?>
            </Concepts>
        </ImplicitData>
    </Profile>
    <Profile name="categories">
        <ImplicitData>
            <Concepts>
<?php foreach ($categories as $cat) { ?>
                <Concept key="<?php echo isset($cat->name) ? $cat->name : $cat->cat_name; ?>" value="<?php echo ((isset($cat->count) ? $cat->count : $cat->category_count *100)/$cat_max)/100; ?>" from="<?php echo $url; ?>" updated="<?php echo $date; ?>"/>
<?php } ?>
            </Concepts>
        </ImplicitData>
    </Profile>
    <Profile name="links">
        <ExplicitData>
            <Concepts>
<?php foreach ($links as $link) { ?>
                <Source key="<?php echo $link->link_url ?>" name="<?php echo $link->link_name ?>" value="<?php echo $link->link_rating != 0 ? $link->link_rating*100/9/100 : "1.0"; ?>" type="text/html" from="<?php echo $url; ?>" updated="<?php echo $date; ?>">
                    <Author key="<?php echo $link->link_name ?>" value="1.0" from="<?php echo $url; ?>" updated="<?php echo $date; ?>" />
                </Source>
<?php } ?>
            </Concepts>
        </ExplicitData>
    </Profile>
</Body>
</APML>
<?php
exit;
  }
}
?>