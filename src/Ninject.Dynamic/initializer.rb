# Note that in the code below I intentionally chose not to use metaprogramming
# I may change my mind later but at the moment I don't see the need for it
# and metaprogramming just for the sake of it .. I have my doubts about that :)
# I also tried to avoid/limit the use of instance_eval as much as possible,
# when it has been used it has to do with the DSL to make it more human readable.

include Ninject::Activation
include Ninject::Activation::Providers
include Ninject::Dynamic::Activation::Providers
BindingTarget = Ninject::Planning::Bindings::BindingTarget

class Symbol
  
  def to_binding_target
    @@binding_target_mappings ||= {
      :type => BindingTarget.type,
      :self => BindingTarget.self,
      :provider => BindingTarget.provider,
      :method => BindingTarget.method,
      :constant => BindingTarget.constant      
    }
    @@binding_target_mappings[self]
  end
  
  def to_scope
    case self
    when :transient
      System::Func.of(IContext, System::Object).new { |context| nil }
    when :singleton
      System::Func.of(IContext, System::Object).new { |context| context.kernel }
    when :thread
      System::Func.of(IContext, System::Object).new { |context| System::Threading::Thread.current_thread }
    when :request
      System::Func.of(IContext, System::Object).new { |context| System::Web::HttpContext.current }
    end
  end
  
end

module System
  class Type
    def to_creation_callback
      StandardProvider.get_creation_callback self
    end
    
    def to_provider_callback
      System::Func.of(IContext, IProvider).new do |context|
        Ninject::ResolutionExtensions.get(context, self)
      end
    end
    
  end
  
  class Object
    def to_provider_callback      
      System.Func.of(IContext, IProvider).new { |context| self }
    end
    
    def to_constant_callback
      System::Func.of(IContext, IProvider).new do |context|
        ConstantProvider.new self
      end
    end
    
    def to_binding_parameter(name)
      Ninject::Parameters::Parameter.new name, self, true
    end
    
    def to_property_value(name)
      Ninject::Parameters::PropertyValue.new name, self, true
    end
    
    def to_constructor_argument(name)
      Ninject::Parameters::ConstructorArgument.new name, self, true
    end
  end
  
  class String
    def to_scope
      self.to_s.to_scope
    end
  end
end

class String
  def to_scope
    self.to_sym.to_scope
  end
end

class Proc
  def to_ruby_proc_callback
    System::Func.of(IContext, IProvider).new do |context|
      RubyProcProvider.new self
    end
  end
  
  def to_scope
    b = self
    System::Func.of(IContext, System::Object).new { |context| b.call(context) }
  end
  
  def to_binding_parameter(name)
    b = self
    callback = System::Func.of(IContext, System::Object).new { |context| b.call(context) }
    Ninject::Parameters::Parameter.new name, callback, true
  end
  
  def to_property_value(name)
    b = self
    callback = System::Func.of(IContext, System::Object).new { |context| b.call(context) }
    Ninject::Parameters::PropertyValue.new name, callback, true
  end
  
  def to_constructor_argument(name)
    b = self
    callback = System::Func.of(IContext, System::Object).new { |context| b.call(context) }
    Ninject::Parameters::ConstructorArgument.new name, callback, true
  end
end

class Class
  def to_provider_callback
    self.to_clr_type.to_provider_callback
  end
  
  def to_creation_callback
    self.to_clr_type.to_creation_callback
  end
end

class Hash
  def has_name?
    def has_name?
      self[:name].is_a?(String) || self[:name].is_a?(System::String) || self[:name].is_a?(Symbol) ||
      self[:name.to_s].is_a?(String) || self[:name.to_s].is_a?(System::String) || self[:name.to_s].is_a?(Symbol)
    end
  end
end

module Ninject
  
  module Planning
    
    module Bindings
      class RubyBindingBuilder
        
        def initialize(config={})
          raise ArgumentError.new "You have to provide a config with a service to build a binding" unless config.has_key? :service
          @config = config
        end
        
        def to_create_a_binding
          @binding = @binding.new @config[:service].to_clr_type
          yield if block_given?
          @binding
        end 
        
        def add_provider_callback
          case 
          when @config[:to] == :self || @config[:to].nil?
            @config[:target] = :self
            @binding.provider_callback = @binding.service.to_creation_callback
          when @config[:to].is_a?(Class)
            @binding.provider_callback = @config[:to].to_creation_callback
            @config[:target] = :type
          when @config[:provider]
            @binding.provider_callback = @config[:provider].to_provider_callback
            @config[:target] = :provider
          when @config[:method]
            @binding.provider_callback = @config[:method].to_ruby_proc_callback
            @config[:target] = :method
          when @config[:constant]
            @binding.provider_callback = @config[:constant].to_constant_callback
            @config[:target] = :constant
          end
        end
        
        def set_metadata
          @binding.metadata.name = System::String.intern(@config[:name].to_s.to_clr_string) if @config.has_name?
          @config[:meta].each do |k, v|
            @binding.metadata.set k.to_s.to_clr_string, v.to_s.to_clr_string
          end if @config[:meta] and @config[:meta].is_a?(Hash)
        end
        
        def set_target
          @binding.target = (@config[:target]||:self).to_sym.to_binding_target
        end
        
        def add_scope
          @config[:as] ||= @config[:scoped]
          @binding.scope_callback = @config[:as].to_scope if @config[:as]
        end
        
        def add_parameters
          parameters = config[:parameter] || config[:parameters] || {}
          parameters.each do |name, value|
            @binding.parameters.add value.to_binding_parameter(name)
          end
        end
        
        def add_constructor_arguments
          arguments = config[:constructor_argument] || config[:constructor_arguments] || {}
          arguments.each do |name, value|
            @binding.parameters.add value.to_constructor_argument(name)
          end
        end
        
        def add_property_values
          values = config[:property_value] || config[:property_values] || {}
          values.each do |name, value|
            @binding.parameters.add value.to_property_value(name)
          end
        end 
        
        def build
          to_create_a_binding do 
            add_provider_callback 
            add_scope 
            set_metadata 
            add_parameters
            add_constructor_arguments
            add_property_values
            set_target 
          end 
        end
        
      end
      
    end
    
  end
  
end

class NinjectInitializer
  
  attr_accessor :bindings
  
  def initialize
    @bindings = []
  end
  
  def bind(service_type, config={})
    config[:service] ||= service_type
    @bindings << RubyBindingBuilder.new(config).build
  end
  
  def clear_bindings
    @bindings=[]
  end
  
end

def to_configure_ninject(&b)
  raise ArgumentError.new("You need to provide a block for the initializer to work") if b.nil?
  initializer = NinjectInitializer.new
  b.call(initializer)
  initializer.bindings
end

#to_configure_ninject do |ninject|
#  ninject.bind IServiceA,
#  :to => ServiceA,
#  :as => :singleton,
#  :meta => { :type => "superservice" },
#  :name => "aaaaa",
#  :parameter => [{ :name => "my_param", :value => lambda { |context| "param_value" }  }],
#  :constructor_arguments => [{:name => :const_arg, :value => 56 }],
#  :property_values => [{:name => :property_name, :value => 94 }],
#  :on_activation => lambda { |obj| obj.do_some_work },
#  :on_deativated => lambda { |obj| obj.do_some_cleanup },
#  :when => {
#    :provider => lambda { |context| "a value" },
#    :injected_into => ServiceB,
#    :target_has => AnAttribute,
#    :member_has => AnAttribute,
#    :class_has => AnAttribute
#  ]
#end

#to_configure_ninject do |ninject|
#  ninject.bind IServiceA, :to => ServiceA, :as => :singleton do
#    meta { :type => "superservice" },
#    name  "aaaaa",
#    with :parameter => [{ :my_param => lambda { |context| "param_value" }  }],
#         :constructor_arguments => [{:const_arg => 56 }],
#         :property_values => [{:property_name => 94 }],
#    on_activation { |obj| obj.do_some_work },
#    on_deativated { |obj| obj.do_some_cleanup },
#    when :provider => lambda { |context| "a value" },
#         :injected_into => ServiceB,
#         :target_has => AnAttribute,
#         :member_has => AnAttribute,
#         :class_has => AnAttribute
#    
#end