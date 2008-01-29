module AutomataParser
  class Stack

    def initialize
      @data = Array.new
    end

    def push elem
      @data.push elem
    end

    def pop
      @data.delete(@data.last)
    end

    def top
      @data.last
    end

    def empty?
      @data.empty?
    end

  end
end
