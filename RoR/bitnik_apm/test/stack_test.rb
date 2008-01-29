require 'automata_parser/stack'
require 'test/unit'
require File.dirname(__FILE__) +'/test_helper'

include AutomataParser

class StackTest < Test::Unit::TestCase

  def setup
    @stack = Stack.new
  end

  def test_stack
    assert_equal(true,@stack.empty?)
    assert_nothing_raised do
      @stack.push 1
      @stack.push 2
    end

    assert_equal(2,@stack.top)
    assert_equal(2,@stack.pop)
    assert_equal(1,@stack.top)
    assert_equal(false,@stack.empty?)

    assert_equal(1,@stack.pop)
    assert_equal(nil,@stack.top)
    assert_equal(true,@stack.empty?)

    assert_equal(nil,@stack.pop)
  end

end
