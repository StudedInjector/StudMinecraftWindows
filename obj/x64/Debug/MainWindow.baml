   M S B A M L   `   `   `  ����  StudInjector    DotNet.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  StudInjector��5   	    � WindowStyle$
 None=�5   	   $	��400��5   	   $	��600��5   	    J� SnapsToDevicePixels .  � WindowStartupLocation$ CenterScreen=�5	   	    � AllowsTransparency . 
������  J� Opacity$ 0Q�5   	    � 	Resources 5       �� 
TargetType   ModernButton �� Property	 �� Value
 �� 
TargetType ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName��%�  &	        �� 5   
   " ��6   .�5      ��  �@�Cursor$
	 Hand��6'   �� 5       � ��
	 �����6+   �� 5       � ��	 �	#FF2196F36+   �� 5       � ��$	 0q�60   �� 5       � ��	 5      �� 5      "
 ��6&   .R�5      �� $ border��6!   #��z��61   $ 5��6[   ._�5      �� $��Center=�6/   $��Center=�6L   5       5      P� 5       �@J�IsMouseOver . . �5   "   �� $ border��6)    � ��	 �	#FF1976D26S   5      P� 5        � �� . . �5!   "   �� $ border��6)    � ��	 �	#FF0D47A16S   5"      P� 5#       � {� .  . �5$   "   ��  � ��	 �	#FF607D8B6?   5%      5'      5)      5*      5+      .��5/      �� $	 9.5��6   ���	#FF1A1A1A$��10q�67    .  � UseLayoutRounding .  J� Effect 50   
   3  -System.Windows.Media.Effects.DropShadowEffect  51         ShadowDepth$ 1Q�6      
BlurRadius$ 15Q�6/      Color$ Black��6?      Opacity$	 0.5Q�6M      RenderingBias$ Quality=�6[   ._�54   
   � .+�55      �� -   5         ��Name$ MainContent��55      $��Center=�6.   $��Center=�6I   .+�56      �� $
��Stud��6   $��32(�6(   ���	#FFE0E0E0$��Light"�6M   $��0,0,0,30q�6`   $��Center=�6r   �� 57      -   5         ��Name$ InjectButton��57      $��Inject��6/   #��[  6@   $	��140��6f   $��35��6r   6~   58      �� 5:      -   5        $ ProgressContent��5:      $��Center=�62   $��Center=�6M   $ 0Q�6j   .+�5;      �� -   5         ��Name$ 
StatusText��5;      $��downloading dll��60   $��20(�6G   ���	#FF808080$��0,0,0,15q�6l   $��Center=�6~   �� 5<      $ 4��6   $	��300��6*   $��8��66   ���	#2FFFFFFF . ._�5=      � -   5         �Name$ ProgressBar��5=   "   ���	#FF2196F3
������ $��0q�6g    J� Clip 5>      �� 5?       �� Rect$ 	0,0,300,8�60    �� RadiusX$ 4Q�6A    �� RadiusY$ 4Q�6M   5A      5B      �� 5C      $��v0.5::a��6   $��14(�6+   ���	#FF505050$��0,15,0,0q�6P   $��Center=�6b   5D      �� 5F      -   5        $ CloseButton��5F      $	��✕��6*   $��28��66   $��28��6A   $��Right=�6M   $	��Top=�6i   $��	0,12,12,0q�6�   ���	#FF808080
������ ��5G      �� 5H      " ��6   .�5I      ��  �@�Cursor$
	 Hand��63   �� 5J       � ��	 5K      �� 5L   "   "
 ��62   .R�5M   &   �� #��z��6-   $ 14��6W   ._�5N   *   �� $��Center=�6;   $��Center=�6X   5O   '   5P   #   5R        �� Triggers  5S      P� 5T       �@J�IsMouseOver . . �5U   "   ��  � ��	 �	#FF3333336?   �� 5V   "    � ��
	 �����6?   5W      5Y      5[      5\      5]      5^      