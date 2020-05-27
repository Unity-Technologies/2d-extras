# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [1.5.0-preview] - 2020-02-14
### Added
- Added CONTRIBUTING.md
- Updated LICENSE.md

### Added
- [PrefabRandomBrush] Split functionality of PrefabBrush to PrefabBrush and PrefabRandomBrush
- [PrefabBrush/PrefabRandomBrush] Add Erase Any Objects toggle to choose between erasing any Objects or Objects in the Brush

### Changed
- Consolidated menus items

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
- [RuleTile] Delete GetMatchingNeighboringTiles(), no longer get nearby Tiles in advance, the performance is affected. (may be changed to cache later)
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
