module AutomataParser

  module Constants
    ANYTHING = '_AutomataParser_*_'
  end

  class GraphVertexWrapper

    include AutomataParser::Constants

    attr_accessor :name

    def initialize(name,arcs)
      @arcs = arcs
    end

    def with(label)
      @arcs.each do |arc|
        if(arc[label]!=nil)
          return arc[label]
        end
      end

      for arc in @arcs
        if(arc[ANYTHING]!=nil)
          return arc[ANYTHING]
        end
      end

      return nil
    end
  end

  class DirectedGraph

    def initialize(graph)

      @vertices_map = Hash.new
      @inverted_vertices_map = Hash.new

      graph.each do |a|
        #output arcs
        if(@vertices_map[a[0]]==nil)
          @vertices_map[a[0]] = Array.new
        end
        @vertices_map[a[0]].push({ a[2] => a[1]})
        #input arcs
        if(@inverted_vertices_map[a[1]]==nil)
          @inverted_vertices_map[a[1]]= Array.new
        end
        @inverted_vertices_map[a[1]].push({ a[2] => a[0]})
      end
    end

    def add_arc transiction
      #output arcs
      trans = @vertices_map[transiction[0]]
      if(trans!=nil)
        found = false
        trans.each do |t|
          if(t[transiction[2]]==transiction[1])
            found = true
          end
        end
        if(found==false)
          @vertices_map[transiction[0]].push({ transiction[2] => transiction[1]})
        end
      else
        @vertices_map[transiction[0]] = Array.new
        @vertices_map[transiction[0]].push({ transiction[2] => transiction[1]})
      end

      #input arcs
      trans = @inverted_vertices_map[transiction[1]]
      if(trans!=nil)
        found = false
        trans.each do |t|
          if(t[transiction[2]]==transiction[0])
            found = true
          end
        end
        if(found==false)
          @vertices_map[transiction[1]].push({ transiction[2] => transiction[0]})
        end
      else
        @vertices_map[transiction[1]] = Array.new
        @vertices_map[transiction[1]].push({ transiction[2] => transiction[0]})
      end

    end

    def del_arc transiction
      #output arcs
      trans = @vertices_map[transiction[0]]
      if(trans!=nil)
        found = nil
        trans.each do |t|
          if(t[transiction[2]]==transiction[1])
            found = t
          end
        end
        if(found!=nil)
          @vertices_map[transiction[0]].delete(found)
        end
      end

      #input arcs
      trans = @inverted_vertices_map[transiction[1]]
      if(trans!=nil)
        found = nil
        trans.each do |t|
          if(t[transiction[2]]==transiction[0])
            found = t
          end
        end
        if(found!=nil)
          @vertices_map[transiction[1]].delete(found)
        end
      end

    end

    def walk_from(vertex)

      arcs = @vertices_map[vertex]

      if(arcs==nil)
        return nil
      else
        return build_vertex_wrapper(vertex,arcs)
      end
    end

    private

    def build_vertex_wrapper(name,arcs)
      return GraphVertexWrapper.new(name,arcs)
    end

  end
end
