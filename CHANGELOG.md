# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [4.1.0] - 2024-09-18

### Fixed

- [GameObjectBrush] Allow expansion of SceneRoot Grid foldout when clicking on label
- [GridInformation] Fix exception when serializing GridInformation component if component is part of a Prefab
- Remove dependency on com.unity.ugui

## [4.0.2] - 2023-08-21

### Fixed

- [GameObjectBrush] Use cell offset to determine location of GameObject when painting and erasing

## [4.0.1] - 2023-04-26

### Fixed

- [GameObjectBrush] Set HideFlags of instantiated GameObject to HideFlags.None when painting
- [GridInformation] Fix serialization of GridInformationKey/Value

## [4.0.0] - 2023-03-07

### Added

- [RuleTile] Add RotatedMirror rule which checks neighbors using both the mirror and rotation Rule in addition to the
  standard rotation Rule

### Fixed

- [GameObjectBrush] Validate size of GameObjectBrush when it changes

## [4.0.0-pre.3] - 2022-11-23

### Fixed

- [TintBrush] Replace obsolete method calls.
- [TintBrushSmooth] Replace obsolete method calls.
- [GameObjectBrush] Fix placement of GameObjects for Hexagon Layouts with Anchor
- [GameObjectBrush] Align rotation and flip to 2D View in Editor
- [RandomBrush] Use default color and transform when painting over with RandomBrush

### Added

- [AnimatedTileEditor] Add field to change TileAnimationFlags

### Changed

- [AnimatedTileEditor] Moved to Unity.2d.Tilemap.Extras.Editor

## [4.0.0-pre.2] - 2022-11-03

### Changed

- [RandomBrush] Add RenderStaticPreview for RandomBrush
- [TintBrush] Add RenderStaticPreview for TintBrush
- [GameObjectBrush] Add RenderStaticPreview for GameObjectBrush
- [GameObjectBrush] Add shouldSaveBrushForSelection for GameObjectBrush
- [GridBrush] Add icons for brushes

### Fixed

- [LineBrush] Do not serialize lineStartActive

## [4.0.0-pre.1] - 2022-10-04

- Update version to 4.0.0-pre.1 for Unity 2023.1

### Fixed

- [GridInformation] Implement IEquatable for GridInformationKey
- [PrefabRandomBrush] Fix possible NullReferenceException in PrefabRandomBrush
- [GameObjectBrush] Fix placement of GameObjects when Cell Gap is set

## [3.0.2] - 2022-04-01

### Fixed

- [RuleOverrideTile] -Mark RuleOverrideTile InstanceTile as dirty when overriding RuleTile for the RuleOverrideTile
  changes
- [RuleOverrideTile] -Fix undo for RuleOverrideTile when overriding RuleTile has changed
- [RuleTileEditor] -Fix height for ReorderableList when extending the view for marking Rules

## [3.0.1] - 2022-03-03

### Fixed

- [AnimatedTileEditor] -Fix undo when setting number of Sprites for Animated Tile
- [RuleTile] -Fix data for custom container fields not being transferred in RuleOverrideTiles overriding a Custom Rule
  Tile
- [RuleTileEditor] -Fix undo when setting number of Rules for Rule Tile
- [RuleTileEditor] -Use different text color for Extend Neighbors with dark and light skin

## [3.0.0] - 2021-08-06

- Update version to 3.0.0 for Unity 2022.1

### Changed

- [GameObjectBrush] Add canChangePosition
- [GameObjectBrush] Use GridLayout from BrushTarget if it has one
- [HexagonalRuleTile] Fix GetOffsetPositionReverse
- [RuleOverrideTile] Create instance Tile on override
- [RuleTile] Add scripting documentation
- [RuleTileEditor] Add drag and drop rect for Sprites to create initial TilingRules
- [RuleTileEditor] Add field to change number of TilingRules
- [RuleTileEditor] Add blank space to the end of the Rule list
- [RuleTileEditor] Add undo for changes
- [AnimatedTileEditor] Add undo for changes
- [TintBrush] Convert cell positions to world positions based on the Grid used
- [TintBrush] Add k_ScaleFactor for better precision when painting on non-rectangular Tilemaps

### Fixed

- [RuleTile] Fixed error in RuleTileEditor when removing all Rules and adding a new Rule

## [2.2.0] - 2021-06-01

### Changed

- [RuleTileEditor] Add tooltips to fields
- Add required package dependencies

## [2.1.0] - 2021-05-06

### Changed

- [RuleTile] Improve performance of RuleTile caching
- [RuleTileEditor] Allow non-public fields with the SerializeField attribute as custom fields for RuleTile
- Make U2DExtrasPlaceholder internal

### Fixed

- [RuleTileEditor] Fix exception when adding a new Rule when no Rule is selected

## [2.0.0] - 2021-03-17

- Update version to 2.0.0

## [2.0.0-pre.3] - 2021-02-19

- [HexagonalRuleTile] Fix issue with mirror rule
- [RuleTile] Add min and max animation speedup
- [RuleOverrideTile] Fix import issue when upgrading from a previous version of RuleOverrideTile
- [RuleTileEditor] Add new rule below selected rule in RuleTileEditor
- [RuleTileEditor] Add dropdown to duplicate Rule

## [2.0.0-pre.2] - 2020-11-26

### Changed

- Update documentation
- Add contribution notice in README.md
- Update Third Party Notices.md
- [PrefabBush] Add pick
- [PrefabBush] Add tooltip for "Erase Any Objects" field
- [PrefabBrush][GameObjectBrush] Account for Anchor when using GetObjectsInCell in PrefabBrush and GameObjectBrush
- [CustomRuleTileScript] Allow Custom Rule Tile template script to be created regardless of where template script is
  installed (from a package or in the project)

## [2.0.0-pre.1] - 2020-10-14

- Update version to 2.0.0-pre.1

## [1.6.2-preview] - 2020-09-25

### Changed

- [RuleTile/RuleOverrideTile/AdvancedRuleOverrideTile] Renamed Game Object to GameObject
- [RuleTile] Fix menu order for RuleOverrideTile
- [RuleOverrideTile] Fix menu order for RuleOverrideTile
- [AdvancedRuleOverrideTile] Fix Rule toggle for AdvancedRuleOverrideTile
- [GameObjectBrush] Use correct position when ClearSceneCell
- [GameObjectBrush] Update cells when size changes
- [GameObjectBrush] Clear cell for Prefabs
- [LineBrush] Clear previews from base.OnPaintSceneGUI
- [PrefabBrush] Fix box erase

## [1.6.1-preview] - 2020-08-11

### Changed

- Update samples

## [1.6.0-preview] - 2020-05-27

### Changed

- Updated for Unity 2020.1
- [GameObjectBrush] Allow painting, erasing and picking on Tile Palette
- [GameObjectBrush] Add Paint on Scene view to GameObjectBrush
- [PrefabBush] Add BoxFill to PrefabBrush
- [PrefabBush] Add Rotation to PrefabBrush
- Consolidated menu items

## [1.5.0-preview] - 2020-02-14

### Added

- Added CONTRIBUTING.md
- Updated LICENSE.md

### Added

- [PrefabRandomBrush] Split functionality of PrefabBrush to PrefabBrush and PrefabRandomBrush
- [PrefabBrush/PrefabRandomBrush] Add Erase Any Objects toggle to choose between erasing any Objects or Objects in the
  Brush

### Changed

- Consolidated menu items

### Fixed

- [WeightedRandomTile] Fixed WeightedRandomTile messing up Random.seed!

## [1.4.0] - 2020-01-07

### Added

- [RuleTile / HexagonalRuleTile / IsometricRuleTile / RuleOverrideTile] Added Asset Preview for TilingRules
- [RuleTile] Hidden Rule field
- [CustomRuleTile] Support custom field of Object type
- [CustomRuleTile] Support HideInInspector, DontOverride attributes
- [RuleOverrideTile] Move advanced mode to AdvancedRuleOverrideTile
- [RuleOverrideTile] Add GameObject overrides
- [RuleOverrideTile] List height lessen
- [RuleOverrideTile] Don't override null sprite
- [RuleOverrideTile] Add static preview
- [AdvancedRuleOverrideTile] List GUI simplify
- [RuleOverrideTile / AdvancedRuleOverrideTile] Show unused overrides
- [RuleOverrideTile / AdvancedRuleOverrideTile] Support multiple inheritance
- [RuleOverrideTile / AdvancedRuleOverrideTile] Prevent circular reference
- [AnimatedTile] Added Animation Start Frame which helps to calculate the Animation Start Time for a given Tilemap

### Fixed

- [RuleTile] Fixed RuleTile InstantiatedGameObject rotation/scale
- [RuleTile] Fixed override tiles have not update when default properties changed
- [AdvancedRuleOverrideTile] Fix override rule lost reference when source rule reorder
- [PrefabBrush] Use WorldToCell comparison when getting GameObjects using PrefabBrush

## [1.3.1] - 2019-11-06

### Changed

- [RuleTile] Simplified
- [RuleTile] Caching all RuleTile neighbor positions for Tilemap to speedup refresh affected tiles

### Fixed

- [RuleTile] Fix remote positions missing of MirrorXY (#148)
- [HexagonalRuleTile] Fix ApplyRandomTransform() of HexagonalRuleTile missing MirrorXY case
- [RuleOverrideTile] Fix RuleOverrideTile does not refresh when add/remove rule
- [RuleTile] Fix random rotation calculation mistake
- [RuleTile] Fix cache data will not update when rule change

## [1.3.0] - 2019-11-01

### Changed

- [RuleTile] changed from using index to using position.
- [RuleTile] Additional storage rule position.
- [RuleTile] Delete DontCare rule.
- [RuleTile] Rule list increased Extend Neighbor toggle. When selected, it will increase the rule range that can be set.
- [RuleTile] No longer fixed to checking around 8 rules.
- [RuleTile] RefreshTile() will refresh affected remote Tiles.
- [RuleTile] Delete GetMatchingNeighboringTiles(), no longer get nearby Tiles in advance, the performance is affected. (
  may be changed to cache later)
- [IsometricRuleTile] Rewrite.
- [HexagonalRuleTile] Rewrite.
- [LineBrush] Fix for Tiles disappear after selection and drag with LineBrush
- [RuleTile] Add MirrorXY Transform Rule

## [1.2.0] - 2019-10-17

### Changed

- [PrefabBrush] Erase GameObjects at target position before painting
- [RuleTileEditor] Made RuleTileEditor and children public
- [RuleTile] Roll back m_Self to this.
- [RuleOverrideTile] Remove m_OverrideSelf property.
- [RuleOverrideTile] Inherit custom properties from custom RuleTile.
- [RuleOverrideTile] Change m_RuntimeTile to m_InstanceTile.

## [1.1.0] - 2019-08-23

### Changed

- Validate Gap and Limit for GroupBrush
- Fix z iterator for RandomBrush
- Check randomTileSets on addToRandomTiles
- Add Anchor to GameObjectBrush and PrefabBrush

## [1.1.0] - 2019-03-22

### Changed

- Copy GameObject when copying TilingRule in RuleOverrideTile

## [1.1.0] - 2019-03-08

### Added

- Added com.unity.2d.tilemap as a dependency of com.unity.2d.tilemap.extras

### Changed

- Custom Grid Brushes have been updated to the UnityEditor.Tilemaps namespace

## [1.0.0] - 2019-01-02

### This is the first release of Tilemap Extras, as a Package
