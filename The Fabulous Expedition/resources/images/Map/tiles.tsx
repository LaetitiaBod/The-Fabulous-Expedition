<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.10" tiledversion="1.11.0" name="tiles" tilewidth="250" tileheight="384" tilecount="15" columns="0">
 <grid orientation="orthogonal" width="1" height="1"/>
 <tile id="25" type="water">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="001.png" width="250" height="250"/>
  <objectgroup draworder="index" id="3">
   <object id="3" x="0" y="0" width="250" height="250"/>
  </objectgroup>
 </tile>
 <tile id="26" type="island" probability="0.01">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="002.png" width="250" height="250"/>
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="250" height="250"/>
  </objectgroup>
 </tile>
 <tile id="27" type="grass">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="003.png" width="250" height="250"/>
 </tile>
 <tile id="28" type="mountain" probability="0.05">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="004.png" width="250" height="384"/>
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="91" width="250" height="293"/>
   <object id="2" x="0" y="91" width="250" height="293"/>
  </objectgroup>
 </tile>
 <tile id="29" type="forest" probability="0.2">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="005.png" width="250" height="384"/>
 </tile>
 <tile id="30" type="desert">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="006.png" width="250" height="250"/>
 </tile>
 <tile id="31" type="desert_mountain" probability="0.05">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="007.png" width="250" height="384"/>
 </tile>
 <tile id="32" type="ground_volcano">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="008.png" width="250" height="250"/>
 </tile>
 <tile id="33" type="volcano">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="009.png" width="250" height="384"/>
 </tile>
 <tile id="34" type="snow">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="010.png" width="250" height="250"/>
 </tile>
 <tile id="35" type="snow_mountain" probability="0.3">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="011.png" width="250" height="384"/>
 </tile>
 <tile id="36" type="snow_forest" probability="0.4">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="012.png" width="250" height="384"/>
 </tile>
 <tile id="37" type="question_mark">
  <properties>
   <property name="isVisible" type="bool" value="true"/>
  </properties>
  <image source="013.png" width="250" height="250"/>
  <objectgroup draworder="index" id="2">
   <object id="2" x="46" y="22" width="158" height="206"/>
  </objectgroup>
 </tile>
 <tile id="45" type="pyramid">
  <properties>
   <property name="isVisible" type="bool" value="false"/>
  </properties>
  <image source="014.png" width="250" height="250"/>
 </tile>
 <tile id="46">
  <image source="015.png" width="250" height="250"/>
 </tile>
</tileset>
