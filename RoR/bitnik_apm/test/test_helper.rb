$:.unshift(File.dirname(__FILE__) + '/../lib')

require 'test/unit'
RAILS_ENV= "test"

require File.expand_path(File.join(File.dirname(__FILE__),'../../../../config/environment.rb'))

=begin
ActiveRecord::Base.class_eval do
  alias_method :save, :valid?
  def self.columns() @columns ||= []; end

  def self.column(name,sql_type=nil,default=nil,null=true)
    columns << ActiveRecord::ConnectionAdapters::Column.new(name.to_s,default,sql_type,null)
  end
end

module BitnikApmTests
  class BitnikApmModel < ActiveRecord::Base
    column :id, :integer
    column :name, :string

    acts_as_attention_destiny :key_mapping => :name
  end
end
=end
