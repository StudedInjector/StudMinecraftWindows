   M S B A M L   `   `   `  ���� 
  DotNet    DotNet.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  StudInjector�� � WindowStyle$
 None=�$	��400��$	��600�� J� SnapsToDevicePixels .  � WindowStartupLocation$ CenterScreen=� � AllowsTransparency . 
������  J� Opacity$ 0Q� � 	Resources  �� 
TargetType   ModernButton �� Property	 �� Value
 �� 
TargetType ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName��%�  &	        �� " ��.���  �@�Cursor$
	 Hand����  � ��
	 �������  � ��	 �	#FF2196F3��  � ��$	 0q���  � ��	 �� "
 ��.R��� $ border��#��z��$ 5��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ border�� � ��	 �	#FF1976D2P�  � �� . . ��� $ border�� � ��	 �	#FF0D47A1P�  � {� .  . ���  � ��	 �	#FF607D8B.���� $	 9.5�����	#FF1A1A1A$��10q� .  � UseLayoutRounding .  J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 1Q�   
BlurRadius$ 15Q�   Color$ Black��   Opacity$	 0.5Q�   RenderingBias$ Quality=�._�� .+��� -    ��Name$ MainContent��$��Center=�$��Center=�.+��� $
��Stud��$��32(����	#FFE0E0E0$��Light"�$��0,0,0,30q�$��Center=��� -    ��Name$ InjectButton��$��Inject��#��[  $	��140��$��35���� -   $ ProgressContent��$��Center=�$��Center=�$ 0Q�.+��� -    ��Name$ 
StatusText��$��downloading dll��$��20(����	#FF808080$��0,0,0,15q�$��Center=��� $ 4��$	��300��$��8�����	#2FFFFFFF . ._�� -    �Name$ ProgressBar�����	#FF2196F3
������ $��0q� J� Clip ��  �� Rect$ 	0,0,300,8� �� RadiusX$ 4Q� �� RadiusY$ 4Q��� $��v0.5::a��$��14(����	#FF505050$��0,15,0,0q�$��Center=��� -   $ CloseButton��$	��✕��$��28��$��28��$��Right=�$	��Top=�$��	0,12,12,0q����	#FF808080
������ ���� " ��.���  �@�Cursor$
	 Hand����  � ��	 �� "
 ��.R��� #��z��$ 14��._��� $��Center=�$��Center=�  �� Triggers  P�  �@J�IsMouseOver . . ���  � ��	 �	#FF333333��  � ��
	 �����