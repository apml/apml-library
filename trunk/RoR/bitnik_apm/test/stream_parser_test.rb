require 'automata_parser/graph'
require 'automata_parser/parser'
require 'test/unit'
require File.dirname(__FILE__) +'/test_helper'


include AutomataParser

class StreamParserTest < Test::Unit::TestCase

  include AutomataParser::Constants

  XML = <<__END
     <catalog>
      <category name="testCategoryA">
       <game name="test1" id="435">
         <device id="33"/>
         <description>
           Texto descriptivo.
         </description>
      </game>
      <game name="test2" id="436">
         <devices>
            <device id="34"/>
            <device id="35"/>
         </devices>
      </game>
     </category>
    </catalog>
__END

  def setup
    @mapping = [[:init,:categ,"category"],
                [:categ,:game,"game"],
                [:game,:device,"device"],
                [:device,:game,"/device"],
                [:game,:description,"description"],
                [:description,:game,"/description"],
                [:game,:categ,"/game"],
                [:game,:init,"/category"]]
    @parser = AutomataParser::Parser.new(@mapping,:init,self)
    @xml = StringIO.new(XML)
    @categories = Array.new
    @games = Array.new
    @devices = Array.new
    @descriptions = Array.new
  end

  def test_parse
    @parser.parse @xml

    assert_equal(1,@categories.size)
    assert_equal(2,@games.size)
    assert_equal(3,@devices.size)
    assert_equal(1,@descriptions.size)
    assert_equal("testCategoryA",@categories[0].values["name"])
    assert_equal(12,@categories[0].values["id"])

  end

  def found_category(category)
    @categories.push category
    category.values["id"]=12
  end

  def parsed_game(game)
    @games.push game
    if(game.values["id"]=="435")
      assert_equal("test1",game.values["name"])
    end
    if(game.values["id"]=="436")
      assert_equal("test2",game.values["name"])
    end

    assert_equal("category",game.parent.name)
    assert_equal(12,game.parent.values["id"])
  end

  def parsed_description(description)
    @descriptions.push description
    assert_not_nil(description.text)
    assert_equal("game",description.parent.name)
    assert_equal("test1",description.parent.attribute["name"])
  end

  def parsed_device(device)
    @devices.push device
    assert_equal(true,(device.values["id"]=="33" || devices.values["id"]=="34" || devices.values["id"]=="35"))
    assert_equal("game",device.parent.name)
  end
end
