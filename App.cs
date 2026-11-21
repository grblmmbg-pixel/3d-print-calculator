using System;
using System.Windows;
using System.Windows.Markup;

namespace PrintCalc
{
    public class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();

            // Setup dark theme resources
            string xamlResources = @"
<ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
    
    <SolidColorBrush x:Key='DarkBg' Color='#13161F' />
    <SolidColorBrush x:Key='DarkCard' Color='#1E222D' />
    <SolidColorBrush x:Key='DarkBorder' Color='#2D3344' />
    <SolidColorBrush x:Key='DarkText' Color='#FFFFFF' />
    <SolidColorBrush x:Key='DarkTextMuted' Color='#9CA3AF' />
    <SolidColorBrush x:Key='AccentCyan' Color='#38BDF8' />
    <SolidColorBrush x:Key='AccentGreen' Color='#10B981' />

    <!-- TextBox Dark Style -->
    <Style TargetType='{x:Type TextBox}'>
        <Setter Property='Background' Value='#13161F' />
        <Setter Property='Foreground' Value='#FFFFFF' />
        <Setter Property='TextElement.Foreground' Value='#FFFFFF' />
        <Setter Property='BorderBrush' Value='#2D3344' />
        <Setter Property='BorderThickness' Value='1' />
        <Setter Property='CaretBrush' Value='#38BDF8' />
        <Setter Property='SelectionBrush' Value='#06B6D4' />
        <Setter Property='Padding' Value='8,6' />
        <Setter Property='Template'>
            <Setter.Value>
                <ControlTemplate TargetType='{x:Type TextBox}'>
                    <Border Name='border' 
                            Background='{TemplateBinding Background}' 
                            BorderBrush='{TemplateBinding BorderBrush}' 
                            BorderThickness='{TemplateBinding BorderThickness}' 
                            CornerRadius='8'>
                        <ScrollViewer x:Name='PART_ContentHost' Margin='2' />
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property='IsFocused' Value='True'>
                            <Setter TargetName='border' Property='BorderBrush' Value='#38BDF8' />
                        </Trigger>
                        <Trigger Property='IsMouseOver' Value='True'>
                            <Setter TargetName='border' Property='BorderBrush' Value='#4B5563' />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!-- ComboBox Dark Style -->
    <Style TargetType='{x:Type ComboBoxItem}'>
        <Setter Property='Background' Value='#1E222D' />
        <Setter Property='Foreground' Value='#FFFFFF' />
        <Setter Property='TextElement.Foreground' Value='#FFFFFF' />
        <Setter Property='TextBlock.Foreground' Value='#FFFFFF' />
        <Setter Property='Padding' Value='12,10' />
        <Setter Property='Cursor' Value='Hand' />
        <Setter Property='Template'>
            <Setter.Value>
                <ControlTemplate TargetType='{x:Type ComboBoxItem}'>
                    <Border Name='itemBorder' Background='{TemplateBinding Background}' Padding='{TemplateBinding Padding}'>
                        <ContentPresenter TextElement.Foreground='#FFFFFF' TextBlock.Foreground='#FFFFFF' />
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property='IsHighlighted' Value='True'>
                            <Setter TargetName='itemBorder' Property='Background' Value='#2A3040' />
                        </Trigger>
                        <Trigger Property='IsSelected' Value='True'>
                            <Setter TargetName='itemBorder' Property='Background' Value='#10B981' />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style TargetType='{x:Type ComboBox}'>
        <Setter Property='Background' Value='#13161F' />
        <Setter Property='Foreground' Value='#FFFFFF' />
        <Setter Property='TextElement.Foreground' Value='#FFFFFF' />
        <Setter Property='TextBlock.Foreground' Value='#FFFFFF' />
        <Setter Property='BorderBrush' Value='#2D3344' />
        <Setter Property='BorderThickness' Value='1' />
        <Setter Property='Padding' Value='12,10' />
        <Setter Property='Template'>
            <Setter.Value>
                <ControlTemplate TargetType='{x:Type ComboBox}'>
                    <Grid TextElement.Foreground='#FFFFFF' TextBlock.Foreground='#FFFFFF'>
                        <ToggleButton Name='ToggleButton'
                                      Focusable='false'
                                      IsChecked='{Binding Path=IsDropDownOpen, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}'
                                      ClickMode='Press'
                                      Background='{TemplateBinding Background}'
                                      BorderBrush='{TemplateBinding BorderBrush}'
                                      BorderThickness='{TemplateBinding BorderThickness}'>
                            <ToggleButton.Template>
                                <ControlTemplate TargetType='{x:Type ToggleButton}'>
                                    <Border Name='toggleBorder' 
                                            Background='{TemplateBinding Background}' 
                                            BorderBrush='{TemplateBinding BorderBrush}' 
                                            BorderThickness='{TemplateBinding BorderThickness}' 
                                            CornerRadius='8'>
                                        <Grid Margin='10,0,12,0'>
                                            <Path Data='M 0 0 L 5 5 L 10 0 Z' 
                                                  Fill='#9CA3AF' 
                                                  HorizontalAlignment='Right' 
                                                  VerticalAlignment='Center' />
                                        </Grid>
                                    </Border>
                                    <ControlTemplate.Triggers>
                                        <Trigger Property='IsMouseOver' Value='True'>
                                            <Setter TargetName='toggleBorder' Property='BorderBrush' Value='#38BDF8' />
                                        </Trigger>
                                    </ControlTemplate.Triggers>
                                </ControlTemplate>
                            </ToggleButton.Template>
                        </ToggleButton>
                        
                        <!-- Selected Item Display: guaranteed white font and doesn't intercept clicks -->
                        <ContentPresenter Margin='{TemplateBinding Padding}'
                                          Content='{TemplateBinding SelectionBoxItem}'
                                          ContentTemplate='{TemplateBinding SelectionBoxItemTemplate}'
                                          ContentTemplateSelector='{TemplateBinding ItemTemplateSelector}'
                                          IsHitTestVisible='False'
                                          TextElement.Foreground='#FFFFFF'
                                          TextBlock.Foreground='#FFFFFF'
                                          VerticalAlignment='Center' />
                        
                        <Popup Name='Popup'
                               Placement='Bottom'
                               IsOpen='{TemplateBinding IsDropDownOpen}'
                               AllowsTransparency='True'
                               Focusable='False'
                               PopupAnimation='Slide'>
                            <Grid Name='DropDown'
                                  SnapsToDevicePixels='True'
                                  MinWidth='{TemplateBinding ActualWidth}'
                                  MaxHeight='{TemplateBinding MaxDropDownHeight}'>
                                <Border Name='DropDownBorder'
                                        Background='#1E222D'
                                        BorderBrush='#2D3344'
                                        BorderThickness='1'
                                        CornerRadius='8'
                                        Margin='0,4,0,4'>
                                    <ScrollViewer Margin='2' SnapsToDevicePixels='True'>
                                        <StackPanel IsItemsHost='True' 
                                                    KeyboardNavigation.DirectionalNavigation='Contained' 
                                                    TextElement.Foreground='#FFFFFF' 
                                                    TextBlock.Foreground='#FFFFFF' />
                                    </ScrollViewer>
                                </Border>
                            </Grid>
                        </Popup>
                    </Grid>
                    <ControlTemplate.Triggers>
                        <Trigger Property='IsFocused' Value='True'>
                            <Setter Property='BorderBrush' Value='#38BDF8' />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!-- Custom Dark ScrollBar -->
    <Style TargetType='{x:Type ScrollBar}'>
        <Setter Property='Background' Value='#13161F' />
        <Setter Property='Foreground' Value='#2D3344' />
        <Setter Property='Width' Value='10' />
        <Setter Property='Template'>
            <Setter.Value>
                <ControlTemplate TargetType='{x:Type ScrollBar}'>
                    <Grid Background='Transparent'>
                        <Track Name='PART_Track' IsDirectionReversed='true'>
                            <Track.Thumb>
                                <Thumb>
                                    <Thumb.Template>
                                        <ControlTemplate TargetType='{x:Type Thumb}'>
                                            <Border Background='#2D3344' CornerRadius='5' Margin='1' />
                                        </ControlTemplate>
                                    </Thumb.Template>
                                </Thumb>
                            </Track.Thumb>
                        </Track>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

</ResourceDictionary>";

            try
            {
                var dict = (ResourceDictionary)XamlReader.Parse(xamlResources);
                app.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Resource parsing: " + ex.Message);
            }

            var win = new MainWindow();
            app.Run(win);
        }
    }
}
