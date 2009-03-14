require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes

to_configure_ninject do |ninject|
  ninject.bind IWeapon, :to => Sword
  ninject.bind Samurai, :to => :self
end