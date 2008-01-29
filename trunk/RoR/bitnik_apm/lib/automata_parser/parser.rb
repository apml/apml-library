require 'rexml/streamlistener'
require 'rexml/document'

module AutomataParser

  class ParserNode
    attr_accessor :parent
    attr_accessor :son
    attr_accessor :name
    attr_accessor :values
    attr_accessor :text

    def initialize
      @values = Hash.new
    end

    def link node
      @son = node
      node.parent = self
    end
  end

  class Parser
    include REXML::StreamListener

    attr_reader :current_state
    def initialize(mapping,initial_state,caller)
      @graph = DirectedGraph.new(mapping)
      @stack = Stack.new
      @current_state = initial_state
      @caller = caller
    end

    def parse(stream)
      REXML::Document.parse_stream(stream, self)
    end

    def tag_start(name,attrs)
      dst = @graph.walk_from(@current_state).with(name)
      if(dst!=nil)
        tmp = ParserNode.new
        tmp.name = dst
        attrs.each_pair do |key,value|
          tmp.values[key]=value
        end
        unless @stack.empty?
          @stack.top.link tmp
        end
        @stack.push tmp

        @current_state = dst

#        puts "Found #{name} -> #{dst}"
        begin
          eval <<__END
            @caller.found_#{name}(tmp)
__END
        rescue Exception => ex
        end
      end
    end

    def text(text)
      if(@stack.top)
        @stack.top.text = text
      end
    end

    def tag_end(name)
      dst = @graph.walk_from(@current_state).with("/"+name.to_s)
      if(dst!=nil)
        tmp = @stack.pop
        @current_state = dst
#        puts "PARSED:#{name} -> #{dst}"
        begin
          eval <<__END
             @caller.parsed_#{name}(tmp)
__END
        rescue Exception => ex
#          puts ex.message
#          puts ex.backtrace.join("\n")
        end
      end
    end

    def add_rule rule
      @graph.add_arc(rule)
    end

    def del_rule rule
      @graph.del_arc(rule)
    end

  end
end
