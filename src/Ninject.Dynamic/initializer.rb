require 'mscorlib'

include Ninject::Activation
include Ninject::Activation::Providers
include Ninject::Dynamic::Activation::Providers
BindingTarget = Ninject::Planning::Bindings::BindingTarget

def puts(msg)
  System::Console.write_line msg.to_s
end

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
    
    def to_injected_into_condition
      k = self
      fn = lambda do |request| 
        return false if request.target.nil?
        request.target.member.reflected_type == k 
      end
      Ninject::Dynamic::Workarounds.to_request_predicate fn
    end
    
    def to_class_has_condition
      k = self
      fn = lambda do |request| 
        return false if request.target.nil?
        request.target.member.reflected_type.is_defined(k)
      end
      Ninject::Dynamic::Workarounds.to_request_predicate(fn)
    end
    
    def to_member_has_condition
      k = self
      fn = lambda do |request| 
        return false if request.target.nil?
        request.target.member.is_defined(k)
      end
      Ninject::Dynamic::Workarounds.to_request_predicate fn
    end
    
    def to_target_has_condition
      k = self
      fn = lambda do |request| 
        return false if request.target.nil?
        request.target.is_defined(k)
      end
      Ninject::Dynamic::Workarounds.to_request_predicate fn
    end
  end
  
  class Object
    def to_provider_callback      
      System.Func.of(IContext, IProvider).new { |context| self }
    end
    
    def to_constant_callback
      System::Func.of(IContext, IProvider).new do |context|
        ConstantProvider.new(self)
      end
    end
    
    def to_binding_parameter(name)
      Ninject::Parameters::Parameter.new(name.to_s.to_clr_string, self, false)
    end
    
    def to_property_value(name)
      Ninject::Parameters::PropertyValue.new(name.to_s.to_clr_string, self)
    end
    
    def to_constructor_argument(name)
      Ninject::Parameters::ConstructorArgument.new(name.to_s.to_clr_string, self)
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
    b = self
    System::Func.of(IContext, IProvider).new do |context|
      RubyProcProvider.new b
    end
  end
  
  def to_scope
    b = self
    System::Func.of(IContext, System::Object).new { |context| b.call(context) }
  end
  
  def to_binding_parameter(name)
    b = self
    callback = System::Func.of(IContext, System::Object).new { |context| b.call(context) }
    Ninject::Parameters::Parameter.new(name.to_s.to_clr_string, callback, false)
  end
  
  def to_property_value(name)
    b = self
    callback = System::Func.of(IContext, System::Object).new { |context| b.call(context) }
    Ninject::Parameters::PropertyValue.new(name.to_s.to_clr_string, callback)
  end
  
  def to_constructor_argument(name)
    b = self
    callback = System::Func.of(IContext, System::Object).new { |context| b.call(context) }
    Ninject::Parameters::ConstructorArgument.new(name.to_s.to_clr_string, callback)
  end
  
  def to_dotnet_action
    b = self
    System::Action.of(IContext).new { |context| b.call(context.instance) }
  end
  
  def to_when_condition
    Ninject::Dynamic::Workarounds.to_request_predicate self
  end
end

class Class
  def to_provider_callback
    self.to_clr_type.to_provider_callback
  end
  
  def to_creation_callback
    self.to_clr_type.to_creation_callback
  end
  
  def to_injected_into_condition
    self.to_clr_type.to_injected_into_condition
  end
  
  def to_class_has_condition
    self.to_clr_type.to_class_has_condition
  end
  
  def to_member_has_condition
    self.to_clr_type.to_member_has_condition
  end
  
  def to_target_has_condition
    self.to_clr_type.to_target_has_condition
  end
end

class Hash
  def has_name?
    self[:name].is_a?(String) || self[:name].is_a?(System::String) || self[:name].is_a?(Symbol) ||
    self[:name.to_s].is_a?(String) || self[:name.to_s].is_a?(System::String) || self[:name.to_s].is_a?(Symbol)
  end
  
  def to_when_condition
    result = nil
    self.each do |name, value|
      result = value.send("to_#{name}_condition".to_sym)
    end
    result
  end
end

module Ninject
  
  module Planning
    
    module Bindings
      class RubyBindingBuilder
        
        def initialize(config={})
          raise ArgumentError.new("You have to provide a config with a service to build a binding") unless config.has_key?(:service)
          @config = config
        end
        
        def to_create_a_binding
          @binding = Binding.new(@config[:service].to_clr_type)
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
            @binding.metadata.set(k.to_s.to_clr_string, v.to_s.to_clr_string)
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
          return nil if @config[:with].nil?
          parameters = @config[:with][:parameter] || @config[:with][:parameters] || {}
          parameters.each do |name, value|
            @binding.parameters.add(value.to_binding_parameter(name))
          end
        end
        
        def add_constructor_arguments
          return nil if @config[:with].nil?
          arguments = @config[:with][:constructor_argument] || @config[:with][:constructor_arguments] || {}
          arguments.each do |name, value|
            @binding.parameters.add(value.to_constructor_argument(name))
          end
        end
        
        def add_property_values
          return nil if @config[:with].nil?
          values = @config[:with][:property_value] || @config[:with][:property_values] || {}
          values.each do |name, value|
            @binding.parameters.add(value.to_property_value(name))
          end
        end 
        
        def add_on_activation
          handler = @config[:on_activation]||@config[:activation!]
          handler.each { |h| @binding.activation_actions.add h.to_dotnet_action unless h.nil? } if handler.respond_to?(:each)
          @binding.activation_actions.add(handler.to_dotnet_action) unless handler.nil? && !handler.respond_to?(:each)
        end 
        
        def add_on_deactivation
          handler = @config[:on_deactivation]||@config[:deactivation!]
          handler.each { |h| @binding.deactivation_action.add h.to_dotnet_action unless h.nil? } if handler.respond_to?(:each)
          @binding.deactivation_actions.add(handler.to_dotnet_action) unless handler.nil? && !handler.respond_to?(:each)
        end
        
        def add_when_constraint
          wh = @config[:when]   
          @binding.condition = wh.to_when_condition unless wh.nil?          
        end
        
        def build
          to_create_a_binding do 
            add_provider_callback 
            add_scope 
            set_metadata 
            add_parameters
            add_constructor_arguments
            add_property_values
            add_on_activation
            add_on_deactivation
            add_when_constraint
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
  
  def bind(service_type, config={}, &b)
    config[:service] ||= service_type
    @config = config
    instance_eval(&b) unless b.nil?
    @bindings << Ninject::Planning::Bindings::RubyBindingBuilder.new(config).build
  end
  
  def method_missing(message, *args, &b)
    @@posibilities ||=  [:meta, :name, :with, :on_activation, :on_deactivation, :activated, :deactivated, :when, :condition]
    if @@posibilities.include?(message.to_sym)
      key = (message.to_sym == :condition) ? :when : message.to_sym
      @config[key] = args.first if args.size > 0
      @config[key] = lambda { |request| b.call(request) } unless b.nil?
    else
      super #preserve normal behavior
    end
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
