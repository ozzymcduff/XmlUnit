require_relative 'src/.nuget/nuget'

require 'albacore'


task :default => [:all]

desc "Rebuild solution"
build :build do |msb, args|
  msb.prop :configuration, :Debug
  msb.target = [:Rebuild]
  msb.sln = "src/XmlUnit.sln"
end

desc "Install missing NuGet packages."
task :install_packages do
    NuGet::exec("restore src/XmlUnit.sln -source http://www.nuget.org/api/v2/")
end

desc "test using console"
test_runner :test => [:build] do |runner|
  runner.exe = NuGet::nunit_86_path
  d = File.dirname(__FILE__)
  files = [File.join(d,"tests", "csharp", "bin", "Debug", "Tests.dll")]
  runner.files = files 
end

