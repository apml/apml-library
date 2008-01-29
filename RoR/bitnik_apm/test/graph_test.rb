require 'automata_parser/graph'
require 'test/unit'
require File.dirname(__FILE__) +'/test_helper'


include AutomataParser

class DirectedGraphTest < Test::Unit::TestCase

  include AutomataParser::Constants

  def setup
    mapping = [[:a,:a,"a"],
               [:a,:b,"b"],
               [:b,:b,"b"],
               [:b,:a,"a"],
               [:b,:c,"c"],
               [:c,:a,"d"],
               [:c,:c,ANYTHING]]

    @graph = DirectedGraph.new(mapping)
  end

  def test_graph
    assert_equal(:a,@graph.walk_from(:a).with("a"))
    assert_equal(:b,@graph.walk_from(:a).with("b"))
    assert_equal(nil,@graph.walk_from(:a).with("c"))
    assert_equal(:b,@graph.walk_from(:b).with("b"))
    assert_equal(:a,@graph.walk_from(:b).with("a"))
    assert_equal(:c,@graph.walk_from(:b).with("c"))
    assert_equal(nil,@graph.walk_from(:b).with("d"))
    assert_equal(:c,@graph.walk_from(:c).with("a"))
    assert_equal(:c,@graph.walk_from(:c).with("b"))
    assert_equal(:c,@graph.walk_from(:c).with("c"))
    assert_equal(:a,@graph.walk_from(:c).with("d"))
    assert_equal(:c,@graph.walk_from(:c).with("e"))
  end

  def test_arc_removal
    assert_equal(:a,@graph.walk_from(:a).with("a"))
    @graph.del_arc([:a,:a,"a"])
    assert_equal(nil,@graph.walk_from(:a).with("a"))
  end

  def test_arc_addition
    assert_equal(nil,@graph.walk_from(:a).with("v"))
    @graph.add_arc([:a,:v,"v"])
    assert_equal(:v,@graph.walk_from(:a).with("v"))
  end
end
