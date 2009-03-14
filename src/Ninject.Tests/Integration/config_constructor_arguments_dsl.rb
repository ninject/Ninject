require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes

to_configure_ninject do |ninject|
  ninject.bind IWeapon, :to => Knife do
    with :constructor_argument => { :name => "Blunt knife".to_clr_string } 
  end
  ninject.bind IWeapon, :to => Shuriken
  ninject.bind IWarrior, :to => Samurai
end