class BitnikApmGenerator < Rails::Generator::NamedBase


  def manifest

    #Generate some names for classes, etc. This will inflect on
    #the third parameter given to script/generate if given, or use
    #an array of predefined names.
    #camel_name, under_name, plural_name = args[0] ? inflect_names(args[0]) : ['Monkey', 'monkey', 'monkeys']

    #Add generators. The file_name variable contains the second parameter
    #to script/generate, so with "script/generate acts_as_monkey foo" it
    #will be "foo".
    record do |m|
      case file_name

        when 'setup'
        #Load the 'model.rb' template and parse it using ERb. Values in the
        #:assigns hash will be available in the template as local variables.
        #The second argument is the filename (in relation to RAILS_ROOT) where
        #the file will be written.
        #m.template 'attention_origin.rb', File.join('app', 'models', "attention_origin.rb")

        #Generate the migration as well, unless the --skip-migration
        #option is given.
        generate_migrations(m)
        File.open(File.join('config','initializers','mime_types.rb'),"a") do |file|
          file << 'Mime::Type.register "application/xml+apml", :apml'
        end

        when 'test_infraestructure'
        m.migration_template 'test_migration.rb', File.join('db', 'migrate'),
        {:migration_file_name => "create_apm_test_infraestructure"}
        m.template 'bitnik_apm_foo_object.rb', File.join('app', 'models', "bitnik_apm_foo_object.rb")
        m.template 'bitnik_apm_foo_consumer.rb', File.join('app', 'models', "bitnik_apm_foo_consumer.rb")
        else
          puts "Could not recognise action \"#{file_name}\"."

      end
    end
  end


  #Describe how to use the generator. This will be shown before USAGE
  #on "scritp/generate acts_as_monkey" (with no <action>)
  def banner
    "Usage: #{$0} #{spec.name} <action>"
  end


  protected

    #Break this out into a method, as it's used more than once
    def generate_migrations(m)
      m.migration_template 'acts_as_attention_concept_migration.rb', File.join('db', 'migrate'),
      {:migration_file_name => "create_attention_concepts"}
    end
end
