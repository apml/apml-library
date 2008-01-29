class AttentionReferenceAuthor
  attr_accessor :key
  attr_accessor :value
  attr_accessor :from
  attr_accessor :updated_at
  attr_accessor :attention_destiny_reference
end

class AttentionDestinyReference < ActiveRecord::Base
  belongs_to :attention_profile

  def author
    author = AttentionReferenceAuthor.new
    author.key = author_key
    author.value = author_value
    author.from = author_from
    author.updated_at = author_updated_at
    author.attention_destiny_reference = self
    return author
  end

  def author=(author)
    author_key = author.key
    author_value = author.value
    author_from = author.from
    author_updated_at = author.updated_at
  end
end
