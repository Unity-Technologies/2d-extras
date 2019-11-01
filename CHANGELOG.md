# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [1.3.0] - 2019-11-01
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
