require 'test/unit'
require File.dirname(__FILE__) +'/test_helper'
require 'apm_parser'
require 'ruby-debug'
class BitnikApmTest < Test::Unit::TestCase

  def setup
  end

  def test_attention_generator
    BitnikApmFooObject.destroy_all
    AttentionDestiny.destroy_all

    bafo = BitnikApmFooObject.new
    bafo.name = 'test'
    bafo.save!

    assert_equal(1,AttentionDestiny.count)
    assert_equal(bafo,AttentionDestiny.find(:all).first.attention_generator)
    assert_equal(bafo.attention_destiny, AttentionDestiny.find(:all).first)
  end

  def test_attention_generator_deletion
    BitnikApmFooObject.destroy_all
    AttentionDestiny.destroy_all

    bafo = BitnikApmFooObject.new
    bafo.name = 'test'
    bafo.save!

    assert_equal(1,AttentionDestiny.count)
    assert_equal(bafo,AttentionDestiny.find(:all).first.attention_generator)
    assert_equal(bafo.attention_destiny, AttentionDestiny.find(:all).first)

    bafo.destroy
    assert_equal(0,AttentionDestiny.count)
    assert_equal(0,BitnikApmFooObject.count)
  end

  def test_attention_source
    BitnikApmFooConsumer.destroy_all
    AttentionPoint.destroy_all
    AttentionDestiny.destroy_all
    BitnikApmFooObject.destroy_all
    AttentionProfile.destroy_all
    AttentionDestinyReference.destroy_all

    bafo = BitnikApmFooObject.new
    bafo.name = 'test'
    bafo.key = 'test'
    bafo.save!

    bafc = BitnikApmFooConsumer.new
    bafc.name = "test"
    bafc.save!

    assert_equal(1,AttentionDestiny.count)
    assert_equal(bafo,AttentionDestiny.find(:all).first.attention_generator)
    assert_equal(bafo.attention_destiny, AttentionDestiny.find(:all).first)
    assert_equal(1,BitnikApmFooConsumer.count)
    assert_equal(1,AttentionProfile.count)
    prof = AttentionProfile.find(:all).first
    assert_equal("default",prof.name)
    assert_equal(bafc,prof.attention_source)

    assert_nothing_raised do
      bafc.pay_attention_to bafo,20, :implicit
    end

    assert_equal(1,AttentionPoint.count)

    assert_nothing_raised do
      bafc.pay_attention_to bafo, 5, :implicit
    end

    assert_equal(2,AttentionPoint.count)

    bafc_2 = BitnikApmFooConsumer.new
    bafc_2.name = "test"
    bafc_2.save

    assert_nothing_raised do
      bafc_2.pay_attention_to bafo, 5, :implicit
    end


    assert_equal(3,AttentionPoint.count)

    assert_equal(1,bafc.attention_destinies(:implicit).size)
    assert_equal(bafo,bafc.attention_destinies(:implicit).first)

    assert_equal(2,bafc.attention_points_for(bafo,:implicit).size)
    assert_equal(25,bafc.attention_total_value_for(bafo,:implicit))

    bafo_2 = BitnikApmFooObject.new
    bafo_2.name = "bafo_2"
    bafo_2.key = "test_2"
    bafo_2.save!

    assert_equal(2,BitnikApmFooObject.count)
    assert_equal(2,AttentionDestiny.count)

    bafc.pay_attention_to bafo_2, 3, :explicit

    assert_equal(4,AttentionPoint.count)

    assert_equal(1,bafc.attention_destinies(:explicit).size)
    assert_equal(1,bafc.attention_destinies(:implicit).size)
    assert_equal(0,bafc.attention_points_for(bafo_2,:implicit).size)
    assert_equal(1,bafc.attention_points_for(bafo_2,:explicit).size)
    assert_equal(3,bafc.attention_total_value_for(bafo_2,:explicit))
    assert_equal(0,bafc.attention_total_value_for(bafo_2,:implicit))

    visited_bafo = false
    visited_bafo_2 = false

    bafc.build_attention_profile_mapping(:implicit).each_pair do |attention_dst,value|
      if(attention_dst==bafo)
        visited_bafo = true
        assert_equal(value,25)
      end
      if(attention_dst==bafo_2)
        visited_bafo_2 = true
      end
    end

    assert_equal(true,visited_bafo)
    assert_equal(false,visited_bafo_2)

    visited_bafo = false
    visited_bafo_2 = false

    bafc.build_attention_profile_mapping(:explicit).each_pair do |attention_dst,value|
      if(attention_dst==bafo)
        visited_bafo = true
      end
      if(attention_dst==bafo_2)
        visited_bafo_2 = true
        assert_equal(value,3)
      end
    end

    assert_equal(false,visited_bafo)
    assert_equal(true,visited_bafo_2)

    bafc.create_attention_profile :test

    ref = AttentionDestinyReference.new
    ref.key="http://test.com"
    ref.name="test"
    ref.from="test"
    ref.author_key="test"
    ref.mime_type="application/rss+xml"
    ref.author_value = 0.9
    ref.author_from="test"
    ref.author_updated_at = Date.today
    ref.from = "test"
    ref.save!

    assert_equal(1,AttentionDestinyReference.count)
    bafc.pay_attention_to(ref,10,:implicit)
    assert_equal(0,bafc.attention_points_for(ref,:explicit).size)
    assert_equal(1,bafc.attention_points_for(ref,:implicit).size)
    assert_equal(10,bafc.attention_points_for(ref,:implicit)[0].value)


    author = AttentionReferenceAuthor.new
    author.key="testAuthor"
    author.value="10"
    author.from="test"
    author.updated_at = Time.now

    ref2 = AttentionDestinyReference.new
    ref2.key="http://test2.com"
    ref2.name="test2"
    ref2.from="test2"
    ref2.mime_type="application/rss+xml"
    ref2.author_key="test2"
    ref2.author_value = 0.9
    ref2.author_from="test2"
    ref2.author_updated_at = Date.today
    ref2.from = "test2"
    ref2.save!

    assert_equal(2,AttentionDestinyReference.count)

    bafc.pay_attention_to(ref2,12,:explicit)
    assert_equal(1,bafc.attention_points_for(ref2,:explicit).size)
    assert_equal(12,bafc.attention_points_for(ref2,:explicit)[0].value)
    assert_equal(0,bafc.attention_points_for(ref2,:implicit).size)

    assert_equal(3,AttentionProfile.count)
    assert_equal(2,bafc.attention_profiles.size)

    bafc.pay_attention_to(bafo_2,20,:implicit,:test)
    assert_equal(0,bafc.attention_points_for(bafo_2,:implicit).size)
    assert_equal(1,bafc.attention_points_for(bafo_2,:implicit,:test).size)
    assert_equal(20,bafc.attention_points_for(bafo_2,:implicit,:test)[0].value)
    assert_equal(48, bafc.compute_total_attention_value)

    assert_equal(1,bafc.attention_reference_destinies(:implicit).size)
    assert_equal(ref,bafc.attention_reference_destinies(:implicit)[0])
    assert_equal(1,bafc.attention_reference_destinies(:explicit).size)
    assert_equal(ref2,bafc.attention_reference_destinies(:explicit)[0])

    bafc.build_attention_profile_mapping(:implicit,:test).each_pair do |attention_dst,value|
      if(attention_dst==bafo_2)
        assert_equal(value,20)
      end
    end

    bafc.build_implicit_attention_profile_mapping(:test).each_pair do |attention_dst,value|
      if(attention_dst==bafo_2)
        assert_equal(value,20)
      end
    end

    found_ref = false
    found_ref2 = false

    bafc.build_attention_reference_profile_mapping(:implicit).each_pair do |attention_ref,value|
      if(attention_ref==ref)
        assert_equal(value,10)
        found_ref=true
      end
    end

    bafc.build_attention_reference_profile_mapping(:explicit).each_pair do |attention_ref,value|
      if(attention_ref==ref2)
        assert_equal(value,12)
        found_ref2 = true
      end
    end

    assert_equal(true,found_ref)
    assert_equal(true,found_ref2)

    xml = nil
    assert_nothing_raised do
      xml = bafc.to_apml

      puts xml
    end

    doc = Bitnik::Apm::ApmlDocument.new(StringIO.new(xml))

    assert_equal("0.6",doc.version)
    assert_equal("APML file",doc.title)
    assert_equal("apm bintik ruby on rails plugin",doc.generator)
    assert_not_nil(doc.date_created.xmlschema)
    assert_equal(2,doc.profiles.keys.size)
    assert_equal(1,doc.profiles["default"].implicit_concepts.keys.size)
    assert_equal("default",doc.profiles["default"].name)
    assert_equal("test",doc.profiles["default"].implicit_concepts["test"].key)
    assert_equal("0.454545454545455",doc.profiles["default"].implicit_concepts["test"].value)
    assert_equal("bitnik.apml.plugin",doc.profiles["default"].implicit_concepts["test"].from)
    assert_not_nil(doc.profiles["default"].implicit_concepts["test"].updated)
    assert_equal(1,doc.profiles["default"].explicit_concepts.keys.size)
    assert_equal(1,doc.profiles["test"].implicit_concepts.keys.size)
    assert_equal(0,doc.profiles["test"].explicit_concepts.keys.size)
    assert_equal(1,doc.profiles["default"].implicit_sources.keys.size)
    assert_equal(1,doc.profiles["default"].explicit_sources.keys.size)
    assert_equal("http://test.com",doc.profiles["default"].implicit_sources["http://test.com"].key)
    assert_equal("test",doc.profiles["default"].implicit_sources["http://test.com"].name)
    assert_equal("0.181818181818182",doc.profiles["default"].implicit_sources["http://test.com"].value)
    assert_equal("application/rss+xml",doc.profiles["default"].implicit_sources["http://test.com"].type)
    assert_equal("test",doc.profiles["default"].implicit_sources["http://test.com"].from)
    assert_not_nil(doc.profiles["default"].implicit_sources["http://test.com"].updated)
    assert_not_nil(doc.profiles["default"].implicit_sources["http://test.com"].author)
    assert_equal("test",doc.profiles["default"].implicit_sources["http://test.com"].author.key)
    assert_not_nil(doc.profiles["default"].explicit_sources["http://test2.com"])

  end

  def test_wrapper_methods
    BitnikApmFooConsumer.destroy_all
    AttentionPoint.destroy_all
    AttentionDestiny.destroy_all
    BitnikApmFooObject.destroy_all
    AttentionProfile.destroy_all

    bafo = BitnikApmFooObject.new
    bafo.name = 'test'
    bafo.save!

    bafc = BitnikApmFooConsumer.new
    bafc.name = "test"
    bafc.save!

    assert_equal(1,AttentionDestiny.count)
    assert_equal(bafo,AttentionDestiny.find(:all).first.attention_generator)
    assert_equal(bafo.attention_destiny, AttentionDestiny.find(:all).first)
    assert_equal(1,BitnikApmFooConsumer.count)
    assert_equal(1,AttentionProfile.count)
    prof = AttentionProfile.find(:all).first
    assert_equal("default",prof.name)
    assert_equal(bafc,prof.attention_source)

    bafc.pay_explicit_attention_to bafo, 10
    assert_equal(bafo,bafc.explicit_attention_destinies.first)
    assert_equal(1,bafc.explicit_attention_points_for(bafo).size)

    bafc.pay_implicit_attention_to bafo, 10
    assert_equal(bafo,bafc.implicit_attention_destinies.first)
    assert_equal(1,bafc.implicit_attention_points_for(bafo).size)

    assert_equal(10,bafc.explicit_attention_total_value_for(bafo))
    assert_equal(10,bafc.implicit_attention_total_value_for(bafo))

    mapping = nil
    assert_nothing_raised do
      mapping = bafc.build_explicit_attention_profile_mapping
    end

    assert_equal(1,mapping.keys.size)

    mapping = nil
    assert_nothing_raised do
      mapping = bafc.build_implicit_attention_profile_mapping
    end

    assert_equal(1,mapping.keys.size)

  end

  def test_attention_destiny_references
    AttentionDestinyReference.destroy_all

    ref = AttentionDestinyReference.new
    ref.key="http://test.com"
    ref.name="test"
    ref.from="test"
    ref.author_key="test"
    ref.author_value = 0.9
    ref.author_from="test"
    ref.author_updated_at = Date.today
    ref.is_explicit = true
    ref.is_implicit = false
    ref.from = false

    assert_nothing_raised do
      ref.save!
    end

    ref.reload

    author = ref.author

    assert_equal("test",author.key)
    assert_equal("0.9",author.value)
    assert_equal("test",author.from)
    assert_not_nil(author.updated_at)
    assert_equal(ref,author.attention_destiny_reference)

    nauth = AttentionReferenceAuthor.new
    nauth.key="test2"
    nauth.value=0.5
    nauth.updated_at = Date.today
    nauth.from="test2"

    ref.author = nauth
    assert_nothing_raised do
      ref.save!
    end

    ref.reload

    assert_equal("test2",nauth.key)
    assert_equal(0.5,nauth.value)
    assert_equal("test2",nauth.from)
    assert_not_nil(nauth.updated_at)

  end
end
